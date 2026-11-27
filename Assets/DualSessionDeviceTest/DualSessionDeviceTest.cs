using System;
using System.Collections.Generic;
using System.Linq;
using Sequence.Adapter;
using Sequence.Authentication;
using Sequence.EmbeddedWallet;
using Sequence.Utils.SecureStorage;
using UnityEngine;

namespace Sequence.DeviceTest
{
    /// <summary>
    /// Manual on-device test for the guest + second-account flow (issue-tracker #7344 / #7348).
    /// Guest on the default SequenceLogin, existing account (Google or email) on an isolated instance, transfer,
    /// app restart, promotion. Built by Assets/DualSessionDeviceTest/Editor/BuildDualSessionDeviceTest.cs.
    /// Every login failure raised by either instance is shown, so a failure on the instance that did not start a
    /// sign-in is visible.
    /// </summary>
    public class DualSessionDeviceTest : MonoBehaviour
    {
        private const string UserNamespace = "user";

        private SequenceLogin _default;
        private SequenceLogin _isolated;
        private EmbeddedWalletAdapter _adapter;
        private SequenceWallet _guest;
        private SequenceWallet _user;
        private bool _busy;
        private string _email = "";
        private string _code = "";
        private Vector2 _scroll;
        private readonly List<string> _log = new List<string>();

        private async void Start()
        {
            _adapter = EmbeddedWalletAdapter.GetInstance();
            _default = SequenceLogin.GetInstance();
            _isolated = SequenceLogin.CreateIsolatedInstance(UserNamespace);

            _default.OnLoginFailed += (error, method, email, methods) => Log($"DEFAULT login failed ({method}): {error}");
            _isolated.OnLoginFailed += (error, method, email, methods) => Log($"ISOLATED login failed ({method}): {error}");
            _isolated.OnSessionWalletCreated += w => { _user = w; Log("ISOLATED signed in " + w.GetWalletAddress()); };
            _isolated.OnMFAEmailSent += e => Log("code sent to " + e);
            _isolated.OnMFAEmailFailedToSend += (e, error) => Log("code send failed: " + error);
            SequenceWallet.OnWalletCreated += w => Log("static OnWalletCreated " + w.GetWalletAddress());

            Log("restoring stored sessions");
            await Run("restore", async () =>
            {
                bool recovered = await _adapter.TryRecoverWalletFromStorage();
                _guest = recovered ? _adapter.Wallet as SequenceWallet : null;
                Log("default restore: " + (recovered ? Text(_adapter.Wallet?.GetWalletAddress()) : "none"));
                var restored = await _isolated.TryToRestoreSessionAsync();
                _user = restored.Wallet as SequenceWallet;
                Log("isolated restore: " + (_user != null ? _user.GetWalletAddress() : "none"));
            });
        }

        private void OnGUI()
        {
            float scale = Screen.width / 540f;
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));
            GUI.color = new Color(0.08f, 0.08f, 0.1f, 1);
            GUI.DrawTexture(new Rect(0, 0, 540, Screen.height / scale), Texture2D.whiteTexture);
            GUI.color = Color.white;
            float width = 540 - 20;
            GUI.skin.label.wordWrap = true;
            GUI.skin.label.fontSize = 14;
            GUI.skin.button.fontSize = 16;
            GUI.skin.textField.fontSize = 16;

            GUILayout.BeginArea(new Rect(10, Screen.safeArea.yMin / scale + 10, width, Screen.height / scale - Screen.safeArea.yMin / scale - 20));
            _scroll = GUILayout.BeginScrollView(_scroll);

            Line("Default (guest)", Text(_guest?.GetWalletAddress()));
            Line("Isolated (user)", Text(_user?.GetWalletAddress()));
            Line("Adapter wallet", Text(_adapter?.Wallet?.GetWalletAddress()));
            Line("Stored default key", StoredAddress(""));
            Line("Stored isolated key", StoredAddress(UserNamespace));
            GUILayout.Label(_busy ? "working…" : "idle");

            GUI.enabled = !_busy;
            if (GUILayout.Button("1. Guest sign-in (default)", GUILayout.Height(44))) _ = Run("guest", GuestSignIn);
            if (GUILayout.Button("2a. Google sign-in (isolated)", GUILayout.Height(44))) { Log("Google sign-in started on isolated"); _isolated.GoogleLogin(); }
            GUILayout.BeginHorizontal();
            _email = GUILayout.TextField(_email, GUILayout.Height(40), GUILayout.Width(width * 0.6f));
            if (GUILayout.Button("2b. Send code", GUILayout.Height(40))) _ = Run("send code", () => _isolated.Login(_email.Trim()));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            _code = GUILayout.TextField(_code, GUILayout.Height(40), GUILayout.Width(width * 0.6f));
            if (GUILayout.Button("2c. Submit code", GUILayout.Height(40))) _ = Run("submit code", () => _isolated.Login(_email.Trim(), _code.Trim()));
            GUILayout.EndHorizontal();
            if (GUILayout.Button("3. Send 0-value tx guest → isolated", GUILayout.Height(44))) _ = Run("transfer", Transfer);
            if (GUILayout.Button("4. Sign out guest", GUILayout.Height(44))) _ = Run("guest sign-out", GuestSignOut);
            if (GUILayout.Button("5. Promote isolated to default", GUILayout.Height(44))) Promote();
            if (GUILayout.Button("6. Sign out current (adapter)", GUILayout.Height(44))) _ = Run("sign out", SignOutCurrent);
            if (GUILayout.Button("Reset: clear both stored sessions", GUILayout.Height(36))) Reset();
            GUI.enabled = true;

            GUILayout.Space(8);
            foreach (string line in Enumerable.Reverse(_log)) GUILayout.Label(line);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private async System.Threading.Tasks.Task GuestSignIn()
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<SequenceWallet>();
            void Created(SequenceWallet w) => tcs.TrySetResult(w);
            _default.OnSessionWalletCreated += Created;
            try
            {
                await _default.ConnectToWaaSAsGuest();
                var done = await System.Threading.Tasks.Task.WhenAny(tcs.Task, System.Threading.Tasks.Task.Delay(30000));
                _guest = done == tcs.Task ? tcs.Task.Result : null;
                Log("guest: " + (_guest != null ? Text(_guest.GetWalletAddress()) : "failed"));
            }
            finally
            {
                _default.OnSessionWalletCreated -= Created;
            }
        }

        private async System.Threading.Tasks.Task Transfer()
        {
            if (_guest == null || _user == null) { Log("need both guest and isolated sessions"); return; }
            TransactionReturn tx = await _guest.SendTransaction(Chain.TestnetArbitrumSepolia,
                new Transaction[] { new RawTransaction(_user.GetWalletAddress(), "0") });
            Log(tx is SuccessfulTransactionReturn ok ? "tx " + ok.txHash : "tx failed: " + (tx as FailedTransactionReturn)?.error);
        }

        private async System.Threading.Tasks.Task GuestSignOut()
        {
            if (_guest == null) { Log("no guest session"); return; }
            bool dropped = await _guest.DropThisSession();
            Log("guest sign-out: " + dropped);
            if (dropped) _guest = null;
        }

        private void Promote()
        {
            if (_user == null) { Log("no isolated session"); return; }
            try
            {
                _isolated.PromoteToDefaultSession(_user);
                Log("promoted " + _user.GetWalletAddress());
                _guest = null;
            }
            catch (Exception e)
            {
                Log("promote failed: " + e.Message);
            }
        }

        private async System.Threading.Tasks.Task SignOutCurrent()
        {
            bool signedOut = await _adapter.SignOut();
            Log("adapter sign-out: " + signedOut);
            if (signedOut) { _guest = null; _user = null; }
        }

        private void Reset()
        {
            var storage = SecureStorageFactory.CreateSecureStorage();
            storage.StoreString(StorageKey(""), "");
            storage.StoreString(StorageKey(UserNamespace), "");
            _guest = null;
            _user = null;
            Log("stored sessions cleared (backend sessions not closed)");
        }

        private async System.Threading.Tasks.Task Run(string name, Func<System.Threading.Tasks.Task> action)
        {
            _busy = true;
            try { await action(); }
            catch (Exception e) { Log($"{name} error: {e.Message}"); }
            finally { _busy = false; }
        }

        private static string StorageKey(string ns) =>
            Application.companyName + "-" + Application.productName + "-" + (string.IsNullOrEmpty(ns) ? "" : ns + "-") + "SessionWallet";

        // Only the wallet address is shown; the stored value also holds the session private key.
        private static string StoredAddress(string ns)
        {
            try
            {
                string raw = SecureStorageFactory.CreateSecureStorage().RetrieveString(StorageKey(ns));
                string[] parts = (raw ?? "").Split('-');
                return parts.Length >= 2 ? parts[1] : "(empty)";
            }
            catch (Exception e)
            {
                return "(error: " + e.Message + ")";
            }
        }

        // Address converts to string implicitly but throws on null.
        private static string Text(Address address) => address == null ? null : address.Value;

        private static void Line(string label, string value) => GUILayout.Label($"{label}: {(string.IsNullOrEmpty(value) ? "-" : value)}");

        private void Log(string line)
        {
            string stamped = DateTime.Now.ToString("HH:mm:ss ") + line;
            Debug.Log("[DualSessionDeviceTest] " + stamped);
            _log.Add(stamped);
        }
    }
}
