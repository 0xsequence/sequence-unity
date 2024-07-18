using System.Threading.Tasks;
using NUnit.Framework;

namespace Sequence.EmbeddedWallet.Tests
{
    public class WalletInteractionsTests
    {
        [TestCase("Hi")]
        [TestCase(@"This is a much
longer message that spans
multiple lines. and has funky characters like this one $ and this one ~ and all of these '~!@#$%^&*()_+{}|:""<>,.?/")]
        public async Task TestSignMessage(string message)
        {
            SequenceLogin login = new SequenceLogin();
            
            login.OnLoginFailed += (error, method, email) =>
            {
                Assert.Fail(error);
            };

            bool checkComplete = false;
            
            SequenceWallet.OnWalletCreated += async wallet =>
            {
                try
                {
                    string signature = await wallet.SignMessage(Chain.Polygon, message);
                    Assert.IsNotNull(signature);
                    
                    IsValidMessageSignatureReturn isValid = await wallet.IsValidMessageSignature(Chain.Polygon, message, signature);
                    Assert.IsNotNull(isValid);
                    Assert.IsTrue(isValid.isValid);
                    checkComplete = true;
                }
                catch (System.Exception e)
                {
                    Assert.Fail(e.Message);
                }
            };
            
            login.GuestLogin();

            while (!checkComplete)
            {
                await Task.Yield();
            }
        }
    }
}