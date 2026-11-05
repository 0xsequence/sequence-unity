using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sequence.Adapter;
using Sequence.Authentication;
using Sequence.Config;
using Sequence.EmbeddedWallet;
using UnityEditor;
using UnityEngine;

namespace Sequence.Editor.Verification
{
    /// <summary>
    /// Batchmode harness for the guest + second-account flow (issue-tracker #7344 / #7348).
    /// Each phase runs in its own editor process, so "restart" is a real process restart.
    ///
    ///   Unity -batchmode -projectPath . -executeMethod Sequence.Editor.Verification.DualSessionVerification.Run -dualPhase setup
    ///   ... -dualPhase restart
    ///   ... -dualPhase final
    ///
    /// Env: SEQ_WAAS_CONFIG_KEY, SEQ_BUILDER_KEY (written into Assets/Resources/SequenceConfig.asset, git-ignored),
    ///      DUAL_STATE_FILE (where phase results are kept between processes).
    /// Not run through the NUnit runner on purpose: SequenceConfig.StoreSessionKey() disables session storage while
    /// a test is executing, and storage is what this harness checks.
    /// </summary>
    public static class DualSessionVerification
    {
        private const string UserNamespace = "user";
        private static readonly List<string> Failures = new List<string>();

        [Serializable]
        private class State
        {
            public string guestAddress;
            public string guestSessionId;
            public string userAddress;
            public string userSessionId;
        }

        public static void Run()
        {
            string phase = GetArg("-dualPhase") ?? "setup";
            Log($"phase={phase} unity={Application.unityVersion}");
            EnsureConfig();

            Task task = phase switch
            {
                "setup" => Setup(),
                "restart" => Restart(),
                "final" => Final(),
                "email-create" => EmailCreate(),
                "email-merge" => EmailMerge(),
                _ => throw new ArgumentException("unknown phase " + phase),
            };

            double deadline = EditorApplication.timeSinceStartup + 900;
            EditorApplication.update += Wait;

            void Wait()
            {
                if (!task.IsCompleted && EditorApplication.timeSinceStartup < deadline) return;
                EditorApplication.update -= Wait;
                if (!task.IsCompleted) Failures.Add("timed out");
                if (task.IsFaulted) Failures.Add("exception: " + task.Exception?.GetBaseException());
                Log(Failures.Count == 0 ? "RESULT: ALL PASS" : "RESULT: FAIL\n  " + string.Join("\n  ", Failures));
                EditorApplication.Exit(Failures.Count == 0 ? 0 : 1);
            }
        }

        private static async Task Setup()
        {
            ClearStoredSessions();
            var adapter = EmbeddedWalletAdapter.GetInstance(); // subscribes to the static wallet events like an app would

            int staticWalletEvents = 0;
            SequenceWallet.OnWalletCreated += _ => staticWalletEvents++;

            // 1. Guest on the default instance (today's integration).
            SequenceLogin guestLogin = SequenceLogin.GetInstance();
            SequenceWallet guest = await LoginAsGuest(guestLogin);
            Check(guest != null, "guest signed in on default instance");
            if (guest == null) return;
            Check(staticWalletEvents == 1, $"default instance raised static OnWalletCreated once (got {staticWalletEvents})");
            Check(Same(adapter.WalletAddress, guest.GetWalletAddress()), "adapter tracks guest wallet");

            // 2. Second account on an isolated instance while the guest stays signed in.
            SequenceLogin userLogin = SequenceLogin.CreateIsolatedInstance(UserNamespace);
            Check(userLogin.IsIsolated && !ReferenceEquals(userLogin, guestLogin), "isolated instance is separate from singleton");
            SequenceWallet user = await LoginAsGuest(userLogin);
            Check(user != null, "second account signed in on isolated instance");
            if (user == null) return;

            Check(staticWalletEvents == 1, $"isolated instance did not raise static OnWalletCreated (count {staticWalletEvents})");
            Check(Same(adapter.WalletAddress, guest.GetWalletAddress()), "adapter still on guest after second sign-in");
            Check(!Same(guest.GetWalletAddress(), user.GetWalletAddress()), "contexts point at different wallets");
            Check(await HasSession(guest), "guest session still valid on backend after second sign-in");
            Check(await HasSession(user), "second session valid on backend");

            // 3. Storage isolation.
            Check(StoredAddress("").Equals(guest.GetWalletAddress(), StringComparison.OrdinalIgnoreCase), "default storage key holds guest session");
            Check(StoredAddress(UserNamespace).Equals(user.GetWalletAddress(), StringComparison.OrdinalIgnoreCase), "namespaced storage key holds second session");

            // 4. Transfer from guest to the address returned by the authenticated session.
            TransactionReturn tx = await guest.SendTransaction(Chain.TestnetArbitrumSepolia,
                new Transaction[] { new RawTransaction(user.GetWalletAddress(), "0") });
            if (tx is SuccessfulTransactionReturn ok)
            {
                Log("tx " + ok.txHash);
                Check(!string.IsNullOrEmpty(ok.txHash), "guest sent tx to second wallet");
            }
            else
            {
                Check(false, "guest sent tx to second wallet: " + (tx as FailedTransactionReturn)?.error);
            }

            SaveState(new State
            {
                guestAddress = guest.GetWalletAddress(), guestSessionId = guest.SessionId,
                userAddress = user.GetWalletAddress(), userSessionId = user.SessionId,
            });
        }

        private static async Task Restart()
        {
            State s = LoadState();
            var adapter = EmbeddedWalletAdapter.GetInstance();

            // 5. Both contexts restore independently in a fresh process.
            bool recovered = await adapter.TryRecoverWalletFromStorage();
            Check(recovered && Same(adapter.WalletAddress, s.guestAddress), "default instance restores guest after restart");
            var guest = adapter.Wallet as SequenceWallet;

            SequenceLogin userLogin = SequenceLogin.CreateIsolatedInstance(UserNamespace);
            var restoredUser = await userLogin.TryToRestoreSessionAsync();
            var user = restoredUser.Wallet as SequenceWallet;
            Check(user != null && Same(user.GetWalletAddress(), s.userAddress), "isolated instance restores second session after restart");
            Check(Same(adapter.WalletAddress, s.guestAddress), "isolated restore did not switch adapter");
            if (guest == null || user == null) return;

            // 6. Finish the switch: end guest, promote second session.
            Check(await guest.DropThisSession(), "guest session closed");
            Check(await HasSession(user), "second session unaffected by guest sign-out");
            userLogin.PromoteToDefaultSession(user);
            Check(Same(adapter.WalletAddress, s.userAddress), "adapter switched to promoted wallet");
            Check(StoredAddress("").Equals(s.userAddress, StringComparison.OrdinalIgnoreCase), "default storage key now holds promoted session");
            Check(string.IsNullOrEmpty(RawStored(UserNamespace)), "namespaced storage key cleared");
        }

        private static async Task Final()
        {
            State s = LoadState();
            var adapter = EmbeddedWalletAdapter.GetInstance();

            // 7. After promotion the plain default flow restores the promoted account.
            bool recovered = await adapter.TryRecoverWalletFromStorage();
            Check(recovered && Same(adapter.WalletAddress, s.userAddress), "default instance restores promoted account after restart");
            if (recovered)
            {
                Check(await adapter.SignOut(), "promoted account signed out");
            }
            ClearStoredSessions();
        }

        [Serializable]
        private class EmailState
        {
            public string emailWallet;
        }

        private static string Email => Environment.GetEnvironmentVariable("DUAL_EMAIL");
        private static string EmailStateFile => StateFile + ".email";

        // Existing account: sign this email in once so it owns a wallet.
        private static async Task EmailCreate()
        {
            ClearStoredSessions();
            SequenceLogin login = SequenceLogin.GetInstance();
            var (wallet, error) = await LoginWithEmail(login, null);
            Check(wallet != null, "email account created: " + error);
            if (wallet == null) return;
            Log("email wallet " + wallet.GetWalletAddress());
            File.WriteAllText(EmailStateFile, JsonUtility.ToJson(new EmailState { emailWallet = wallet.GetWalletAddress() }));
            Check(await wallet.DropThisSession(), "email session closed");
            ClearStoredSessions();
        }

        // TapNation flow: guest live, authenticate the existing email account in an isolated instance, transfer, switch.
        private static async Task EmailMerge()
        {
            var existing = JsonUtility.FromJson<EmailState>(File.ReadAllText(EmailStateFile));
            ClearStoredSessions();
            var adapter = EmbeddedWalletAdapter.GetInstance();
            int staticWalletEvents = 0;
            SequenceWallet.OnWalletCreated += _ => staticWalletEvents++;

            SequenceWallet guest = await LoginAsGuest(SequenceLogin.GetInstance());
            Check(guest != null, "guest signed in on default instance");
            if (guest == null) return;
            Log("guest wallet " + guest.GetWalletAddress());

            SequenceLogin userLogin = SequenceLogin.CreateIsolatedInstance(UserNamespace);
            var (user, error) = await LoginWithEmail(userLogin, "000000");
            Check(user != null, "existing email account authenticated after a wrong OTP: " + error);
            if (user == null) return;

            Check(Same(user.GetWalletAddress(), existing.emailWallet), $"authentication returned the existing wallet {existing.emailWallet} (got {user.GetWalletAddress()})");
            Check(!Same(user.GetWalletAddress(), guest.GetWalletAddress()), "existing wallet differs from guest wallet");
            Check(staticWalletEvents == 1, $"isolated email login did not raise static OnWalletCreated (count {staticWalletEvents})");
            Check(Same(adapter.WalletAddress, guest.GetWalletAddress()), "adapter still on guest after email login");
            Check(await HasSession(guest), "guest session valid after email login");
            Check(StoredAddress("").Equals(guest.GetWalletAddress(), StringComparison.OrdinalIgnoreCase), "default storage key still holds guest");
            Check(StoredAddress(UserNamespace).Equals(existing.emailWallet, StringComparison.OrdinalIgnoreCase), "namespaced key holds existing email wallet");

            TransactionReturn tx = await guest.SendTransaction(Chain.TestnetArbitrumSepolia,
                new Transaction[] { new RawTransaction(user.GetWalletAddress(), "0") });
            var ok = tx as SuccessfulTransactionReturn;
            Log("tx " + ok?.txHash + (tx as FailedTransactionReturn)?.error);
            Check(ok != null && !string.IsNullOrEmpty(ok.txHash), "guest sent tx to existing email wallet");
            if (ok == null) return;

            Check(await guest.DropThisSession(), "guest session closed after confirmed transfer");
            userLogin.PromoteToDefaultSession(user);
            Check(Same(adapter.WalletAddress, existing.emailWallet), "adapter switched to existing email wallet");
            Check(await HasSession(user), "email session valid after switch");
            Check(await adapter.SignOut(), "email account signed out");
            ClearStoredSessions();
        }

        // Sends the OTP, optionally submits a wrong code first (must fail without side effects), then the real code
        // read from $DUAL_STATE_FILE.otp.
        private static async Task<(SequenceWallet, string)> LoginWithEmail(SequenceLogin login, string wrongCodeFirst)
        {
            SequenceWallet created = null;
            string lastError = "";
            int failures = 0;
            void Created(SequenceWallet w) => created = w;
            void Failed(string error, LoginMethod m, string email, List<LoginMethod> methods) { failures++; lastError = error; Log("login failed: " + error); }
            void SendFailed(string email, string error) { lastError = error; Log("otp send failed: " + error); }
            login.OnSessionWalletCreated += Created;
            login.OnLoginFailed += Failed;
            login.OnMFAEmailFailedToSend += SendFailed;
            try
            {
                string otpFile = StateFile + ".otp";
                File.Delete(otpFile);
                await login.Login(Email);
                Log("WAITING_FOR_OTP " + Email);

                if (wrongCodeFirst != null)
                {
                    await login.Login(Email, wrongCodeFirst);
                    Check(created == null && failures == 1, $"wrong OTP rejected (failures {failures})");
                }

                double until = EditorApplication.timeSinceStartup + 600;
                while (!File.Exists(otpFile) && EditorApplication.timeSinceStartup < until) await Task.Delay(1000);
                if (!File.Exists(otpFile)) return (null, "no OTP provided");
                string code = File.ReadAllText(otpFile).Trim();
                File.Delete(otpFile);

                await login.Login(Email, code);
                double wait = EditorApplication.timeSinceStartup + 30;
                while (created == null && EditorApplication.timeSinceStartup < wait) await Task.Delay(200);
                return (created, lastError);
            }
            finally
            {
                login.OnSessionWalletCreated -= Created;
                login.OnLoginFailed -= Failed;
                login.OnMFAEmailFailedToSend -= SendFailed;
            }
        }

        private static async Task<SequenceWallet> LoginAsGuest(SequenceLogin login)
        {
            var tcs = new TaskCompletionSource<SequenceWallet>();
            void Created(SequenceWallet w) => tcs.TrySetResult(w);
            void Failed(string error, LoginMethod m, string email, List<LoginMethod> methods)
            {
                Log("login failed: " + error);
                tcs.TrySetResult(null);
            }
            login.OnSessionWalletCreated += Created;
            login.OnLoginFailed += Failed;
            await login.ConnectToWaaSAsGuest();
            SequenceWallet result = await Task.WhenAny(tcs.Task, Task.Delay(30000)) == tcs.Task ? tcs.Task.Result : null;
            login.OnSessionWalletCreated -= Created;
            login.OnLoginFailed -= Failed;
            return result;
        }

        private static async Task<bool> HasSession(SequenceWallet wallet)
        {
            Session[] sessions = await wallet.ListSessions();
            return sessions != null && sessions.Any(x => x.id == wallet.SessionId);
        }

        private static string StorageKey(string ns) =>
            Application.companyName + "-" + Application.productName + "-" + (string.IsNullOrEmpty(ns) ? "" : ns + "-") + "SessionWallet";

        // Editor secure storage is PlayerPrefs (EditorSecureStorage).
        private static string RawStored(string ns) => PlayerPrefs.GetString(StorageKey(ns));

        private static string StoredAddress(string ns)
        {
            string[] parts = RawStored(ns).Split('-');
            return parts.Length >= 2 ? parts[1] : "";
        }

        private static void ClearStoredSessions()
        {
            PlayerPrefs.DeleteKey(StorageKey(""));
            PlayerPrefs.DeleteKey(StorageKey(UserNamespace));
            PlayerPrefs.Save();
        }

        private static void EnsureConfig()
        {
            const string path = "Assets/Resources/SequenceConfig.asset";
            var config = AssetDatabase.LoadAssetAtPath<SequenceConfig>(path);
            if (config == null)
            {
                Directory.CreateDirectory("Assets/Resources");
                config = ScriptableObject.CreateInstance<SequenceConfig>();
                AssetDatabase.CreateAsset(config, path);
            }
            config.WaaSConfigKey = Environment.GetEnvironmentVariable("SEQ_WAAS_CONFIG_KEY") ?? config.WaaSConfigKey;
            config.BuilderAPIKey = Environment.GetEnvironmentVariable("SEQ_BUILDER_KEY") ?? config.BuilderAPIKey;
            config.EditorStoreSessionPrivateKeyInSecureStorage = true;
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        private static string StateFile => Environment.GetEnvironmentVariable("DUAL_STATE_FILE") ?? "Temp/dual-session-state.json";
        private static void SaveState(State s) => File.WriteAllText(StateFile, JsonUtility.ToJson(s));
        private static State LoadState() => JsonUtility.FromJson<State>(File.ReadAllText(StateFile));

        private static bool Same(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

        private static void Check(bool condition, string message)
        {
            Log((condition ? "PASS " : "FAIL ") + message);
            if (!condition) Failures.Add(message);
        }

        private static void Log(string message) => Debug.Log("[DualSession] " + message);

        private static string GetArg(string name)
        {
            string[] args = Environment.GetCommandLineArgs();
            int i = Array.IndexOf(args, name);
            return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
        }
    }
}
