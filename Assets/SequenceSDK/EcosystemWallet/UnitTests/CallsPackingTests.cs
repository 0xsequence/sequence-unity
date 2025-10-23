using System.Numerics;
using NUnit.Framework;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.UnitTests
{
    public class CallsPackingTests
    {
        [Test]
        public void TestPackCalls()
        {
            string expected =
                "0x13006f000000000000000000000000000000000000000000000000000000000000001b000020565d6c6751f7da3a2225f0577a2fc7d4e04d4807e8e2860dfcbc7d20fbbaf800000031b91f19ca38dc8ebbf91db23bfcc6644095e20cd92de776fad23100abcf";
            
            SolidityDecoded decoded = new SolidityDecoded(SolidityDecoded.Kind.Transaction, false,
                new[]
                {
                    new Call(new Address("0x5615deb798bb3e4dfa0139dfa1b3d433cc23b72f"),
                        BigInteger.Parse("27"),
                        "0x565d6c6751f7da3a2225f0577a2fc7d4e04d4807e8e2860dfcbc7d20fbbaf800"
                            .HexStringToByteArray(),
                        BigInteger.Parse("343176436416564113169670386691375668074847034592115607773457142857640911"), false, true, BehaviourOnError.revert)
                }, BigInteger.Parse("0"), BigInteger.Parse("0"),
                "0x",
                "0x0000000000000000000000000000000000000000000000000000000000000000",
                "0x0000000000000000000000000000000000000000000000000000000000000000",
                new Address[] { });
            
            Parented expectedParented = new Parented(new Address[] { },
                new Calls(BigInteger.Zero, BigInteger.Zero, new Call[]
                {
                    new Call(new Address("0x5615deb798bb3e4dfa0139dfa1b3d433cc23b72f"),
                        BigInteger.Parse("27"),
                        "0x565d6c6751f7da3a2225f0577a2fc7d4e04d4807e8e2860dfcbc7d20fbbaf800"
                            .HexStringToByteArray(),
                        BigInteger.Parse("343176436416564113169670386691375668074847034592115607773457142857640911"),
                        false, true, BehaviourOnError.revert)
                }));

            Parented parented = Parented.DecodeFromSolidityDecoded(decoded);
            Assert.IsNotNull(parented);
            Assert.AreEqual(expectedParented, parented);
            
            Calls calls = (Calls)parented.payload;
            Assert.IsNotNull(calls);
            Assert.AreEqual(1, calls.calls.Length);
            Assert.AreEqual(decoded.calls[0], calls.calls[0]);

            byte[] packed = calls.Encode(new Address("0x5615deb798bb3e4dfa0139dfa1b3d433cc23b72f"));
            Assert.IsNotNull(packed);
            
            string packedHex = packed.ByteArrayToHexStringWithPrefix();
            Assert.AreEqual(expected, packedHex);
        }
    }
}