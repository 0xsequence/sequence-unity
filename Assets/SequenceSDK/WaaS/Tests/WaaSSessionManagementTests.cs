using System.Threading.Tasks;
using NUnit.Framework;
using PlayFab;
using PlayFab.ClientModels;
using Sequence.Authentication;
using UnityEngine;

namespace Sequence.EmbeddedWallet.Tests
{
    public class WaaSSessionManagementTests
    {
        [Test]
        public async Task SignInAndOutRepeatedly_Guest()
        {
            try
            {
                SequenceLogin login = SequenceLogin.GetInstance();
                await Task.Delay(100);
                login.OnLoginFailed += (error, method, email) =>
                {
                    Assert.Fail(error);
                };
                int repetitions = 0;
                SequenceWallet.OnWalletCreated += async wallet =>
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

                while (repetitions < 3)
                {
                    await Task.Yield();
                }
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
            
            SequenceLogin login = SequenceLogin.GetInstance();
            await Task.Delay(100);
            login.OnLoginFailed += (error, method, email) =>
            {
                Assert.Fail(error);
            };
            int repetitions = 0;
            SequenceWallet.OnWalletCreated += async wallet =>
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

            while (repetitions < 3)
            {
                await Task.Yield();
            }
        }
    }
}