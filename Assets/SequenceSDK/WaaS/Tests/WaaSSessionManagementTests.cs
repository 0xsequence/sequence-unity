using System;
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
                SequenceLogin login = new SequenceLogin();
                await Task.Delay(100);
                login.OnLoginFailed += (error, method, email, methods) =>
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
        
        [Test] // Todo: fix this test. Note that the behaviour we're trying to test works, see PlayFabLogin. For some reason, the text context breaks it (I haven't figure out why yet)
        public async Task SignInAndOutRepeatedly_PlayFabGuest()
        {
            string titleId = WaaSEndToEndTestConfig.GetConfig().PlayFabTitleId;
            
            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            {
                PlayFabSettings.staticSettings.TitleId = titleId;
            }
            
            SequenceLogin login = new SequenceLogin();
            await Task.Delay(100);
            login.OnLoginFailed += (error, method, email, methods) =>
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

        [Test]
        public async Task TestGetSessionAuthProof()
        {
            var tcs = new TaskCompletionSource<bool>();
            EndToEndTestHarness testHarness = new EndToEndTestHarness();

            testHarness.Login(async wallet =>
            {
                try
                {
                    IntentResponseSessionAuthProof proof = await wallet.GetSessionAuthProof(Chain.ArbitrumNova);
                    Assert.IsNotNull(proof);
                    Assert.False(string.IsNullOrWhiteSpace(proof.wallet));
                    Assert.False(string.IsNullOrWhiteSpace(proof.signature));
                    Assert.False(string.IsNullOrWhiteSpace(proof.message));
                    Assert.False(string.IsNullOrWhiteSpace(proof.network));
                    Assert.False(string.IsNullOrWhiteSpace(proof.signature));
                        
                    tcs.TrySetResult(true);
                }
                catch (System.Exception e)
                {
                    tcs.TrySetException(e);
                }
            }, (error, method, email, methods) =>
            {
                tcs.TrySetException(new Exception(error));
            });
            
            await tcs.Task;
        }
    }
}
