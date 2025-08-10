using System;
using System.Numerics;
using Sequence.ABI;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Signer;
using Sequence.Utils;
using Sequence.Wallet;

namespace Sequence.EcosystemWallet
{
    public class SessionSigner
    {
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
                    ABI.ABI.FunctionSelector("incrementUsageLimit((bytes32,uint256)[])"))
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

        public int FindSupportedPermissionIndex(Call call, SessionsTopology topology)
        {
            var permissions = topology.GetPermissions()?.permissions;
            return permissions is {Length: > 0} ? 0 : -1;
        }

        public SessionCallSignature SignCall(Call call, SessionsTopology topology, BigInteger space, BigInteger nonce)
        {
            var pvKey = _credentials.privateKey;
            var eoaWallet = new EOAWallet(pvKey);
            
            var hashedCall = HashCallWithReplayProtection(call, space, nonce);
            var signedCall = EthSignature.Sign(hashedCall, eoaWallet.privKey);

            var rsy = RSY.UnpackFrom65(signedCall.HexStringToByteArray());

            if (IsExplicit)
            {
                var permissionIndex = 0;
                if (!(call.data.Length > 4 && call.data.Slice(4).ByteArrayToHexStringWithPrefix() ==
                    ABI.ABI.FunctionSelector("incrementUsageLimit")))
                {
                    permissionIndex = FindSupportedPermissionIndex(call, topology);
                    if (permissionIndex == -1)
                        throw new Exception("Invalid permission");
                }
                
                return new ExplicitSessionCallSignature
                {
                    permissionIndex = permissionIndex,
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