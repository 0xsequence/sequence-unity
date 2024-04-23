using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

namespace Sequence.Relayer
{
    public class MockMinterThatCantMintOddAmounts : IMinter
    {
        public event Action<string> OnMintTokenSuccess;
        public event Action<string> OnMintTokenFailed;
        public async Task<string> MintToken(string tokenId, uint amount = 1)
        {
            if (amount == 5)
            {
                return "{some error}";
            }

            if (amount % 2 == 1)
            {
                return "";
            }

            return ReturnValues[Random.Range(0, ReturnValues.Length)];
        }

        private readonly string[] ReturnValues = new[]
        {
            "0x3F96a0D6697e5E7ACEC56A21681195dC6262b06C",
            "0x6F5Ddb00e3cb99Dfd9A07885Ea91303629D1DA94",
            "0xb396CbD9b745Ffc4a9C9A6D43D7957b1350Be153",
            "0x606e6d28e9150D8A3C070AEfB751a2D0C5DB19fa",
            "0x1099542D7dFaF6757527146C0aB9E70A967f71C0",
            "0xc683a014955b75F5ECF991d4502427c8fa1Aa249"
        };
    }
}