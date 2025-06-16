using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.UnitTests
{
    public class TypedDataToSignTests
    {
        private Address[] _addressArray = new Address[]
        {
            new Address("0xe86ff51267fb7a1ff020a04971203a2f57eb5ab0"),
            new Address("0x6b45dcf3e824145375ab46af51334f823837f90c"),
            new Address("0x2d459f147b3fb3ea6e07a4af02201f85aa995aec"),
            new Address("0x66e63215a4994d95930c5d39231d36c23ab03fc0"),
            new Address("0xf1be39a6aa2279e119a65d15c44456827e48f81a"),
            new Address("0xfc9c1687f8ca3e01e4fd1f7a2c07edff2f1657c4"),
            new Address("0xa3d430fb60a138d1740231bc499df3c89011ddfa"),
            new Address("0x2f6b49aaec1fa5b04485da48dacdd0a2f4786e9b"),
            new Address("0x6339f581b1a4be3878c41b84319b9aed756bae7b"),
            new Address("0x66067612abc3abd2c48b5820cef62727f46016cd"),
            new Address("0xc9a9f3bf19c80ecd30277ac1be0ab17dd538db34"),
            new Address("0x6af6509adca19ef0f1c2b183c0a12fbde4f1b3f2"),
            new Address("0xd51d83fc620cf678cce893703e753d693f106403"),
            new Address("0x262db8e0f4a2fcba5fa7199490f1c609ecfcef1c"),
            new Address("0x2205fd44262d7d5ceead32f2f7d9e1aba9c04c9d"),
            new Address("0xbe010524b4818ef8168498fc55813a462defcaf3"),
            new Address("0x5759cd0751fe51e6bb5232a86b4e0e56e32f20fc"),
            new Address("0xd50153fb2144323d989cbbb440670c5a6101dbce"),
            new Address("0x622e3a3da94079fa0cbf994e06768035e8791527"),
            new Address("0x04b8b03dba0d960415ce5177accf9d399ed852eb"),
            new Address("0xb4cb75becc7d6c25a8815d3a31d997105772d12b"),
            new Address("0x6aa42b1d0f63dae3a5bf785101c2d2bf26679463"),
            new Address("0x992c74ff99dabaacd47ddf171747b927bb9318e9"),
            new Address("0x9ff1939e31843cd8df1eb4f9f9b3b4e2688dcdc9"),
            new Address("0x9fe63115bd584e296a71ddae97087ebb2724c6fd"),
            new Address("0x2c99841fb733c81fc049713cf8a5c250c85b92f4"),
            new Address("0xd232072f82e45a6ddbe592f3ffe77b3e0d44fd21")
        };
            
        [Test]
        public void TestCreateFromConfigUpdatePayload()
        {
            TypedDataToSign expected = new TypedDataToSign(
                new Domain("Sequence Wallet", "3", Chain.LocalChain,
                    new Address("0x5615dEB798BB3E4dFa0139dFa1b3D433Cc23b72f")),
                new Dictionary<string, NamedType[]>
                {
                    ["ConfigUpdate"] = new[]
                    {
                        new NamedType("imageHash", "bytes32"),
                        new NamedType("wallets", "address[]")
                    },
                },
                "ConfigUpdate",
                new Dictionary<string, object>()
                {
                    {
                        "imageHash",
                        "0x6810c263f45be5dc8e8e6ffd2ab9bd6f152412edb66111b6f56e39a42c694405"
                    },
                    {
                        "wallets", _addressArray
                    }
                });

            Address fromWallet = new Address("0x5615dEB798BB3E4dFa0139dFa1b3D433Cc23b72f");
            Chain chain = Chain.LocalChain;

            Parented parented = new Parented(_addressArray,
                new ConfigUpdate("0x6810c263f45be5dc8e8e6ffd2ab9bd6f152412edb66111b6f56e39a42c694405"));

            TypedDataToSign result = new TypedDataToSign(fromWallet, chain, parented);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestCreateFromMessagePayload()
        {
            TypedDataToSign expected = new TypedDataToSign(new Domain("Sequence Wallet", "3", Chain.LocalChain,
                new Address("0x5615dEB798BB3E4dFa0139dFa1b3D433Cc23b72f")), new Dictionary<string, NamedType[]>()
            {
                {
                    "Message", new[]
                    {
                        new NamedType("message", "bytes"),
                        new NamedType("wallets", "address[]")
                    }
                }
            }, "Message", new Dictionary<string, object>()
            {
                {
                    "message",
                    "0xc263f45be5dc8e8e6ffd2ab9bd6f152412edb66111b6f56e39a42c69440581f0e86ff51267fb7a1ff020a049"
                },
                { "wallets", _addressArray }
            });
            
            Address fromWallet = new Address("0x5615dEB798BB3E4dFa0139dFa1b3D433Cc23b72f");
            Chain chain = Chain.LocalChain;
            Parented parented = new Parented(_addressArray,
                new Message(new byte[]
                {
                    194, 99, 244, 91, 229, 220, 142, 142, 111,
                    253, 42, 185, 189, 111, 21, 36, 18, 237,
                    182, 97, 17, 182, 245, 110, 57, 164, 44,
                    105, 68, 5, 129, 240, 232, 111, 245, 18,
                    103, 251, 122, 31, 240, 32, 160, 73
                }));
            
            TypedDataToSign result = new TypedDataToSign(fromWallet, chain, parented);
            
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestCreateFromCallsPayload()
        {
            TypedDataToSign expected = new TypedDataToSign(new Domain("Sequence Wallet", "3", Chain.LocalChain,
                new Address("0xd0B2e0C7b8a2D267733B573Bdc87cC73f551b8A4")), new Dictionary<string, NamedType[]>()
            {
                {
                    "Calls", new NamedType[]
                    {
                        new NamedType("calls", "Call[]"),
                        new NamedType("space", "uint256"),
                        new NamedType("nonce", "uint256"),
                        new NamedType("wallets", "address[]")
                    }
                },
                {
                    "Call", new NamedType[]
                    {
                        new NamedType("to", "address"),
                        new NamedType("value", "uint256"),
                        new NamedType("data", "bytes"),
                        new NamedType("gasLimit", "uint256"),
                        new NamedType("delegateCall", "bool"),
                        new NamedType("onlyFallback", "bool"),
                        new NamedType("behaviorOnError", "uint256")
                    }
                }
            }, "Calls", new Dictionary<string, object>()
            {
                {
                    "calls", new EncodeSapient.EncodedCall[]
                    {
                        new EncodeSapient.EncodedCall(new Call(new Address("0x6810c263f45be5dc8e8e6ffd2ab9bd6f152412ed"),
                            BigInteger.Parse("19035696402805033763977015876"),
                            "0xf51267fb7a1ff020a04971203a2f57eb5ab06b45dcf3e824145375ab46af51334f823837f90c2d459f147b3fb3ea6e07a4af02201f85aa995aec66e632"
                                .HexStringToByteArray(), BigInteger.Parse("29"), false, true, BehaviourOnError.abort)),
                        new EncodeSapient.EncodedCall(new Call(new Address("0x7fa9385be102ac3eac297483dd6233d62b3e1496"), BigInteger.Parse("0"),
                            "0x001122".HexStringToByteArray(), BigInteger.Parse("1000000"), false, false,
                            BehaviourOnError.ignore)),
                    }
                },
                { "space", "1266736520029721018202413622017" },
                { "nonce", "55846603721928660" },
                { "wallets", _addressArray }
            });

            Address fromWallet = new Address("0xd0B2e0C7b8a2D267733B573Bdc87cC73f551b8A4");
            Chain chain = Chain.LocalChain;

            Parented parented = new Parented(_addressArray,
                new Calls(BigInteger.Parse("1266736520029721018202413622017"), BigInteger.Parse("55846603721928660"),
                    new Call[]
                    {
                        new Call(new Address("0x6810c263f45be5dc8e8e6ffd2ab9bd6f152412ed"),
                            BigInteger.Parse("19035696402805033763977015876"),
                            "0xf51267fb7a1ff020a04971203a2f57eb5ab06b45dcf3e824145375ab46af51334f823837f90c2d459f147b3fb3ea6e07a4af02201f85aa995aec66e632"
                                .HexStringToByteArray(), BigInteger.Parse("29"), false, true, BehaviourOnError.abort),
                        new Call(new Address("0x7fa9385be102ac3eac297483dd6233d62b3e1496"), BigInteger.Parse("0"),
                            "0x001122".HexStringToByteArray(), BigInteger.Parse("1000000"), false, false,
                            BehaviourOnError.ignore),
                    }));
            
            TypedDataToSign result = new TypedDataToSign(fromWallet, chain, parented);
            
            Assert.AreEqual(expected, result);
        }

        private Address[] _otherAddressArray = new Address[]
        {
            new Address("0xE86Ff51267fb7a1ff020A04971203a2F57eb5AB0"),
            new Address("0x6b45dCf3e824145375aB46Af51334F823837f90C"),
            new Address("0x2D459F147b3fb3ea6E07A4af02201f85Aa995aeC"),
            new Address("0x66e63215A4994D95930c5D39231D36c23Ab03fC0"),
            new Address("0xf1BE39A6AA2279e119A65d15C44456827e48f81a"),
            new Address("0xfc9C1687F8CA3e01e4FD1F7a2C07EDFF2f1657c4"),
            new Address("0xA3D430FB60a138d1740231bC499df3C89011ddFa"),
            new Address("0x2f6B49AaEC1Fa5b04485dA48Dacdd0A2f4786E9b"),
            new Address("0x6339f581b1A4bE3878c41b84319b9AEd756bAE7b"),
            new Address("0x66067612ABC3ABD2c48b5820cef62727F46016Cd"),
            new Address("0xC9A9f3BF19c80ECD30277ac1Be0Ab17dD538dB34"),
            new Address("0x6aF6509ADca19EF0F1c2b183C0A12FBde4F1b3f2"),
            new Address("0xd51d83Fc620CF678ccE893703E753d693f106403"),
            new Address("0x262db8E0F4a2fCbA5fA7199490F1C609ecFCEf1c"),
            new Address("0x2205fD44262D7d5ceEAd32F2F7D9E1aBA9c04C9D"),
            new Address("0xbE010524b4818Ef8168498fc55813A462DefCaF3"),
            new Address("0x5759cD0751fE51e6Bb5232A86b4E0E56e32F20fC"),
            new Address("0xd50153fb2144323d989CBBb440670c5A6101dBcE"),
            new Address("0x622E3a3dA94079fA0CbF994e06768035e8791527"),
            new Address("0x04B8B03dBa0d960415cE5177acCf9D399ED852eb"),
            new Address("0xB4cB75bEcC7D6C25a8815D3a31d997105772d12B"),
            new Address("0x6aA42b1d0f63dAE3a5bF785101c2D2bF26679463"),
            new Address("0x992C74Ff99dAbAacD47dDf171747B927Bb9318E9"),
            new Address("0x9fF1939E31843CD8DF1Eb4f9f9B3B4E2688DCDC9"),
            new Address("0x9Fe63115bD584E296A71dDAe97087EBb2724C6FD"),
            new Address("0x2c99841fB733C81fC049713CF8a5c250c85B92f4"),
            new Address("0xD232072F82e45a6DdBe592f3ffe77B3E0D44fd21")
        };

        [Test]
        public void TestGetSignedPayload()
        {
            TypedDataToSign toSign = new TypedDataToSign(new Address("0x4caf9086ccf2e486f331Bde537A559d9Be7D2fc4"),
                Chain.LocalChain,
                new Parented(_otherAddressArray,
                    new ConfigUpdate("0x6810c263f45be5dc8e8e6ffd2ab9bd6f152412edb66111b6f56e39a42c694405")));

            string expected = "0x2c685e56801c1443071309b0faa1ba71991982226f3faba021a6083d2063afdb";

            string result = toSign.GetSignPayload().ByteArrayToHexStringWithPrefix();
            
            Assert.AreEqual(expected, result);
        }
    }
}