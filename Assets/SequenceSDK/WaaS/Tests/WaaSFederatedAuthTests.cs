using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PlayFab;
using PlayFab.ClientModels;
using Sequence.EmbeddedWallet;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sequence.EmbeddedWallet.Tests
{
    public class WaaSFederatedAuthTests
    {
        private string _email;
        private TaskCompletionSource<bool> _tcs;
        private string _walletAddress;
        SequenceLogin _login;
        private IWallet _wallet;
        private Account _federatedAccount;

        [Test]
        public async Task TestAccountAssociation()
        {
            _login = SequenceLogin.GetInstance();
            SequenceWallet.OnAccountFederated += OnAccountFederated;
            _tcs = new TaskCompletionSource<bool>();
            EndToEndTestHarness testHarness = new EndToEndTestHarness(_login);

            await testHarness.Login(async wallet =>
            {
                try
                {
                    _walletAddress = wallet.GetWalletAddress();
                    _email = WaaSEndToEndTestConfig.GetConfig().PlayFabEmail;
                    string titleId = WaaSEndToEndTestConfig.GetConfig().PlayFabTitleId;

                    if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
                    {
                        PlayFabSettings.staticSettings.TitleId = titleId;
                    }

                    var request = new LoginWithEmailAddressRequest
                    {
                        Email = _email,
                        Password = WaaSEndToEndTestConfig.GetConfig().PlayFabPassword
                    };
                    PlayFabClientAPI.LoginWithEmailAddress(request, PlayFabLoginResult, PlayFabLoginFailed);
                }
                catch (Exception e)
                {
                    _tcs.TrySetException(e);
                }
            }, (error, method, email, methods) =>
            {
                _tcs.TrySetException(new Exception(error));
            });

            await _tcs.Task;
        }

        private void PlayFabLoginResult(LoginResult result)
        {
            LoginWithPlayFab(result.SessionTicket);
        }

        private async Task LoginWithPlayFab(string sessionTicket)
        {
            SequenceWallet.OnAccountFederationFailed += RetryIfAlreadyLinked;
            try
            {
                await _login.FederateAccountPlayFab(PlayFabSettings.staticSettings.TitleId,
                    sessionTicket,
                    _email, _walletAddress);
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("AccountAlreadyLinked"))
                {
                    _tcs.TrySetException(e);
                }
            }
            finally
            {
                SequenceWallet.OnAccountFederationFailed -= RetryIfAlreadyLinked;
            }
        }

        private void RetryIfAlreadyLinked(string error)
        {
            if (error.Contains("AccountAlreadyLinked"))
            {
                _email = Random.Range(0, 9999999).ToString() + _email;
                var request = new LoginWithEmailAddressRequest
                {
                    Email = _email,
                    Password = WaaSEndToEndTestConfig.GetConfig().PlayFabPassword
                };
                PlayFabClientAPI.LoginWithEmailAddress(request, PlayFabLoginResult, PlayFabLoginFailed);
            }
            else
            {
                _tcs.TrySetException(new Exception(error));
            }
        }

        private void PlayFabLoginFailed(PlayFabError error)
        {
            string errorMessage = error.GenerateErrorReport();
            if (errorMessage.Contains("User not found"))
            {
                Debug.Log("User not found, creating new user");
                var request = new RegisterPlayFabUserRequest
                {
                    Email = _email,
                    Password = WaaSEndToEndTestConfig.GetConfig().PlayFabPassword,
                    RequireBothUsernameAndEmail = false
                };
                PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
            }
            else
            {
                _tcs.TrySetException(new Exception(errorMessage));
            }
        }

        private void OnRegisterSuccess(RegisterPlayFabUserResult result)
        {
            try
            {
                Debug.Log("Registered new user with PlayFab - connecting to WaaS now");

                string sessionTicket = result.SessionTicket;
                string titleId = PlayFabSettings.staticSettings.TitleId;
                _login.FederateAccountPlayFab(titleId, sessionTicket, _email, _walletAddress);
            }
            catch (Exception e)
            {
                _tcs.TrySetException(e);
            }
        }

        private void OnRegisterFailure(PlayFabError error)
        {
            string errorMessage = error.GenerateErrorReport();
            _tcs.TrySetException(new Exception(errorMessage));
        }

        private void OnAccountFederated(Account account)
        {
            if (account.email != _email.ToLower())
            {
                _tcs.TrySetException(new Exception($"Emails do not match {_email} != {account.email}"));
            }
            _tcs.SetResult(true);
            SequenceWallet.OnAccountFederated -= OnAccountFederated;
        }

        [Test]
        public async Task TestRemoveFederatedAccount()
        {
            _login = SequenceLogin.GetInstance();
            SequenceWallet.OnAccountFederated += OnAccountFederatedBeforeDeletion;
            _tcs = new TaskCompletionSource<bool>();
            EndToEndTestHarness testHarness = new EndToEndTestHarness(_login);

            await testHarness.Login(async wallet =>
            {
                _wallet = wallet;
                try
                {
                    _walletAddress = wallet.GetWalletAddress();
                    _email = WaaSEndToEndTestConfig.GetConfig().PlayFabEmail;
                    string titleId = WaaSEndToEndTestConfig.GetConfig().PlayFabTitleId;

                    if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
                    {
                        PlayFabSettings.staticSettings.TitleId = titleId;
                    }

                    var request = new LoginWithEmailAddressRequest
                    {
                        Email = _email,
                        Password = WaaSEndToEndTestConfig.GetConfig().PlayFabPassword
                    };
                    PlayFabClientAPI.LoginWithEmailAddress(request, PlayFabLoginResult, PlayFabLoginFailed);
                }
                catch (Exception e)
                {
                    _tcs.TrySetException(e);
                }
            }, (error, method, email, methods) =>
            {
                _tcs.TrySetException(new Exception(error));
            });

            await _tcs.Task;
        }

        private void OnAccountFederatedBeforeDeletion(Account account)
        {
            if (account.email != _email.ToLower())
            {
                _tcs.TrySetException(new Exception($"Emails do not match {_email} != {account.email}"));
            }
            SequenceWallet.OnAccountFederated -= OnAccountFederatedBeforeDeletion;

            _wallet.OnFederatedAccountRemovedComplete += OnFederatedAccountRemoved;
            _federatedAccount = account;
            _wallet.RemoveFederatedAccount(account);
        }

        private void OnFederatedAccountRemoved(string accountId, bool success)
        {
            if (accountId == _federatedAccount.id)
            {
                _tcs.SetResult(true);
            }
            else
            {
                _tcs.TrySetException(new Exception($"Account ids do not match {_federatedAccount.id} != {accountId}"));
            }
            
            _wallet.OnFederatedAccountRemovedComplete -= OnFederatedAccountRemoved;
        }
    }
}
