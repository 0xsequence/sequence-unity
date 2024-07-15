using System.Threading.Tasks;
using NUnit.Framework;
using PlayFab;
using PlayFab.ClientModels;
using Sequence.Authentication;
using Sequence.Wallet;
using SequenceSDK.WaaS;
using UnityEngine;

namespace Sequence.WaaS.Tests
{
    public class WaaSFederatedAuthTests
    {
        [Test]
        public async Task TestAccountAssociation()
        {
            try
            {
                WaaSLogin login = WaaSLogin.GetInstance();
                login.OnLoginFailed += (error, method, email) => { Assert.Fail(error); };
                string email = WaaSEndToEndTestConfig.GetConfig().PlayFabEmail;
                bool accountFederated = false;
                WaaSWallet.OnAccountFederated += account =>
                {
                    Assert.Equals(account.email, email);
                    accountFederated = true;
                };
                WaaSWallet.OnAccountFederationFailed += error => { Assert.Fail(error); };
                WaaSWallet.OnWaaSWalletCreated += async wallet =>
                {
                    string titleId = WaaSEndToEndTestConfig.GetConfig().PlayFabTitleId;

                    if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
                    {
                        PlayFabSettings.staticSettings.TitleId = titleId;
                    }


                    var request = new LoginWithCustomIDRequest
                        { CustomId = "WaaSSessionManagementTests", CreateAccount = true };
                    PlayFabClientAPI.LoginWithCustomID(request,
                        async result =>
                        {
                            await login.FederateAccountPlayFab(PlayFabSettings.staticSettings.TitleId,
                                result.SessionTicket,
                                email);
                        },
                        error => { Assert.Fail(error.ErrorMessage); });
                };
                await login.ConnectToWaaSAsGuest();
                while (!accountFederated)
                {
                    await Task.Yield();
                }
            }
            catch (System.Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}