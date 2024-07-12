using System.Threading.Tasks;
using NUnit.Framework;
using PlayFab;
using PlayFab.ClientModels;
using Sequence.Authentication;
using UnityEngine;

namespace Sequence.WaaS.Tests
{
    public class WaaSSessionManagementTests
    {
        [Test]
        public async Task SignInAndOutRepeatedly_Guest()
        {
            try
            {
                WaaSLogin login = WaaSLogin.GetInstance();
                login.OnLoginFailed += (error, method, email) =>
                {
                    Assert.Fail(error);
                };
                int repetitions = 0;
                WaaSWallet.OnWaaSWalletCreated += async wallet =>
                {
                    if (repetitions < 3)
                    {
                        try
                        {
                            await wallet.DropThisSession();
                            repetitions++;
                            await login.ConnectToWaaSAsGuest();
                        }
                        catch (System.Exception e)
                        {
                            Assert.Fail(e.Message);
                        }
                    }
                };
                await login.ConnectToWaaSAsGuest();
            }
            catch (System.Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
        
        [Test]
        public async Task SignInAndOutRepeatedly_PlayFabGuest()
        {
            string titleId = WaaSEndToEndTestConfig.GetConfig().PlayFabTitleId;
            
            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            {
                PlayFabSettings.staticSettings.TitleId = titleId;
            }

            PlayFabSettings.staticSettings.TitleId = "8F854"; // Todo remove this dev env credential
            
            WaaSLogin login = WaaSLogin.GetInstance();
            login.OnLoginFailed += (error, method, email) =>
            {
                Assert.Fail(error);
            };
            int repetitions = 0;
            WaaSWallet.OnWaaSWalletCreated += async wallet =>
            {
                if (repetitions < 3)
                {
                    await wallet.DropThisSession();
                    repetitions++;
                    await login.ConnectToWaaSAsGuest();
                }
            };
            
            
            var request = new LoginWithCustomIDRequest { CustomId = "WaaSSessionManagementTests", CreateAccount = true};
            PlayFabClientAPI.LoginWithCustomID(request, 
                async result =>
                {
                    await login.ConnectToWaaSViaPlayFab(titleId, result.SessionTicket, "");
                }, 
                error =>
                {
                    Assert.Fail(error.ErrorMessage);
                });
        }
    }
}