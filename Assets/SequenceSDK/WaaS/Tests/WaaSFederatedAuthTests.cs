using System.Threading.Tasks;
using NUnit.Framework;
using PlayFab;
using PlayFab.ClientModels;
using Sequence.EmbeddedWallet;

namespace Sequence.EmbeddedWallet.Tests
{
    public class WaaSFederatedAuthTests
    {
        [Test]
        public async Task TestAccountAssociation()
        {
            try
            {
                SequenceLogin login = SequenceLogin.GetInstance();
                login.OnLoginFailed += (error, method, email) => { Assert.Fail(error); };
                string email = WaaSEndToEndTestConfig.GetConfig().PlayFabEmail;
                bool accountFederated = false;
                SequenceWallet.OnAccountFederated += account =>
                {
                    Assert.Equals(account.email, email);
                    accountFederated = true;
                };
                SequenceWallet.OnAccountFederationFailed += error => { Assert.Fail(error); };
                SequenceWallet.OnWalletCreated += async wallet =>
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