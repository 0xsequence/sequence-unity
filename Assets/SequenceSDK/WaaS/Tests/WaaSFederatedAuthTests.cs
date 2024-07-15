using System.Threading.Tasks;
using NUnit.Framework;
using PlayFab;
using PlayFab.ClientModels;
using Sequence.Authentication;
using Sequence.Wallet;

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
                login.OnLoginFailed += (error, method, email) =>
                {
                    Assert.Fail(error);
                };
                string email = WaaSEndToEndTestConfig.GetConfig().PlayFabEmail;
                WaaSWallet.OnAccountFederated += account =>
                {
                    Assert.Equals(account.email, email);
                };
                WaaSWallet.OnAccountFederationFailed += error =>
                {
                    Assert.Fail(error);
                };
                WaaSWallet.OnWaaSWalletCreated += async wallet =>
                {
                    string titleId = WaaSEndToEndTestConfig.GetConfig().PlayFabTitleId;
            
                    if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
                    {
                        PlayFabSettings.staticSettings.TitleId = titleId;
                    }

                    PlayFabSettings.staticSettings.TitleId = "8F854"; // Todo remove this dev env credential
                    
            
                    var request = new LoginWithCustomIDRequest { CustomId = "WaaSSessionManagementTests", CreateAccount = true};
                    PlayFabClientAPI.LoginWithCustomID(request, 
                        async result =>
                        {
                            PlayFabConnector playFabConnector = new PlayFabConnector(titleId, result.SessionTicket, wallet.SessionId, new EthWallet(), login);
                            await login.InitiateAuth(playFabConnector.AssemblePlayFabInitiateAuthIntent(), LoginMethod.PlayFab);
                            await login.FederateAccount(
                                new IntentDataFederateAccount(playFabConnector.AssemblePlayFabOpenSessionIntent(),
                                    wallet.GetWalletAddress()), LoginMethod.PlayFab, email);
                        }, 
                        error =>
                        {
                            Assert.Fail(error.ErrorMessage);
                        });
                };
                await login.ConnectToWaaSAsGuest();
            }
            catch (System.Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}