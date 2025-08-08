using System;
using System.Linq;
using System.Numerics;
using System.Text;
using Nethereum.Util;
using Sequence.ABI;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Signer;
using Sequence.Utils;
using Sequence.Wallet;

namespace Sequence.EcosystemWallet
{
    public class SessionSigner
    {
        private const string IncrementUsageLimit =
            "{\n    type: 'function',\n    name: 'incrementUsageLimit',\n    inputs: [\n      {\n        name: 'limits',\n        type: 'tuple[]',\n        internalType: 'struct UsageLimit[]',\n        components: [\n          { name: 'usageHash', type: 'bytes32', internalType: 'bytes32' },\n          { name: 'usageAmount', type: 'uint256', internalType: 'uint256' },\n        ],\n      },\n    ],\n    outputs: [],\n    stateMutability: 'nonpayable',\n  }";
        
        public Address ParentAddress { get; }
        public Address Address { get; }
        public Chain Chain { get; }
        public bool IsExplicit { get; }

        public Address IdentitySigner
        {
            get
            {
                if (IsExplicit)
                    throw new Exception("no identity signer for explicit sessions");

                var attestationHash = _credentials.attestation.Hash();
                var pub = EthCrypto.RecoverPublicKey(_credentials.signature.Pack(), attestationHash);
                var pubXY = EthCrypto.PublicKeyXY(pub);
                var address = EthCrypto.AddressFromPublicKey(pubXY);
                
                return new Address(address);
            }
        }
        
        private readonly SessionCredentials _credentials;
        
        internal SessionSigner(SessionCredentials credentials)
        {
            _credentials = credentials;

            ParentAddress = credentials.address;
            Address = new EOAWallet(credentials.privateKey).GetAddress();
            Chain = ChainDictionaries.ChainById[credentials.chainId];
            IsExplicit = credentials.isExplicit;
        }

        public bool IsSupportedCall(Call call, SessionsTopology topology)
        {
            if (IsExplicit)
            {
                if (call.data.Length > 4 &&
                    ByteArrayExtensions.Slice(call.data, 0, 4).ByteArrayToHexStringWithPrefix() ==
                    GetIncrementUsageLimitSelector())
                {
                    return true;
                }

                var permission = FindSupportedPermission(call, topology);
                return true;
            }
            
            return true;
        }

        public Permission FindSupportedPermission(Call call, SessionsTopology topology)
        {
            var permissions = topology.GetPermissions()?.permissions;
            return permissions is {Length: > 0} ? permissions[0] : null;
        }
        
        public string GetIncrementUsageLimitSelector()
        {
            string signature = "incrementUsageLimit((bytes32,uint256)[])";
        
            var sha3 = new Sha3Keccack();
            var hash = sha3.CalculateHash(Encoding.UTF8.GetBytes(signature));
        
            return "0x" + BitConverter.ToString(hash.Take(4).ToArray()).Replace("-", "").ToLowerInvariant();
        }

        public SessionCallSignature SignCall(Call call, BigInteger space, BigInteger nonce)
        {
            var pvKey = _credentials.privateKey;
            var eoaWallet = new EOAWallet(pvKey);
            
            var hashedCall = HashCallWithReplayProtection(call, space, nonce);
            var signedCall = EthSignature.Sign(hashedCall, eoaWallet.privKey);

            var rsy = RSY.UnpackFrom65(signedCall.HexStringToByteArray());

            if (IsExplicit)
            {
                return new ExplicitSessionCallSignature
                {
                    permissionIndex = 0,
                    sessionSignature = rsy
                };
            }
            
            return new ImplicitSessionCallSignature
            {
                attestation = _credentials.attestation,
                identitySignature = _credentials.signature,
                sessionSignature = rsy
            };
        }

        private byte[] HashCallWithReplayProtection(Call call, BigInteger space, BigInteger nonce)
        {
            var chainBytes = BigInteger.Parse(Chain.GetChainId()).ByteArrayFromNumber(32);
            var spaceBytes = space.ByteArrayFromNumber(32);
            var nonceBytes = nonce.ByteArrayFromNumber(32);
            var callHashBytes = call.Hash().HexStringToByteArray();

            var concatenated = ByteArrayExtensions.ConcatenateByteArrays(chainBytes, spaceBytes, nonceBytes, callHashBytes);
            return SequenceCoder.KeccakHash(concatenated);
        }
    }
}