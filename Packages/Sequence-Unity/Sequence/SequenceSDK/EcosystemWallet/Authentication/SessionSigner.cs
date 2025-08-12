using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Provider;
using Sequence.Signer;
using Sequence.Utils;
using Sequence.Wallet;
using UnityEngine;

namespace Sequence.EcosystemWallet
{
    internal class SessionSigner
    {
        private static readonly Address ValueTrackingAddress = new ("0xEeeeeEeeeEeEeeEeEeEeeEEEeeeeEeeeeeeeEEeE");
        
        public Address ParentAddress { get; }
        public Address Address { get; }
        public Chain Chain { get; }
        public EcosystemType Ecosystem { get; }
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
            Ecosystem = (EcosystemType)credentials.ecosystemId;
            IsExplicit = credentials.isExplicit;
        }

        public async Task<bool> IsSupportedCall(Call call, Chain chain, SessionsTopology topology)
        {
            if (Chain != chain)
                return false;
            
            if (IsExplicit)
            {
                if (call.data.Length > 4 &&
                    ByteArrayExtensions.Slice(call.data, 0, 4).ByteArrayToHexStringWithPrefix() ==
                    ABI.ABI.FunctionSelector("incrementUsageLimit((bytes32,uint256)[])"))
                {
                    return true;
                }

                var supportedPermission = FindSupportedPermission(call, topology);
                return supportedPermission.Index >= 0;
            }

            var response = await new SequenceEthClient(chain).CallContract(new object[] {
                new
                {
                    to = call.to,
                    data = GetAcceptImplicitRequestFunctionAbi(call)
                }
            });

            var expectedResult = GenerateImplicitRequestMagic(ParentAddress, _credentials.attestation);
            return response == expectedResult;
        }
        
        private (int Index, Permission Permission) FindSupportedPermission(Call call, SessionsTopology topology)
        {
            var sessionPermissions = topology.GetPermissions(Address);
            if (sessionPermissions == null || ChainDictionaries.ChainById[sessionPermissions.chainId.ToString()] != Chain)
                return (-1, null);

            // TODO: Read current usage limit, use ValueTrackingAddress
            var exceededLimit = call.value > 0 && call.value > sessionPermissions.valueLimit;
            if (exceededLimit)
                return (-1, null);

            var permissionIndex = 0;
            foreach (var permission in sessionPermissions.permissions)
            {
                if (permission.target.Equals(call.to))
                    break;
                
                permissionIndex++;
            }

            return (permissionIndex, sessionPermissions.permissions[permissionIndex]);
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
                    permissionIndex = FindSupportedPermission(call, topology).Index;
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
        
        private string GetAcceptImplicitRequestFunctionAbi(Call call)
        {
            var attestation = _credentials.attestation;
            var attestationData = new Tuple<Address, FixedByte, FixedByte, FixedByte, Byte[], Tuple<string, BigInteger>>(
                attestation.approvedSigner,
                new FixedByte(4, attestation.identityType.Data), new FixedByte(32, attestation.issuerHash.Data), new FixedByte(32, attestation.audienceHash.Data), attestation.applicationData.Data,
                new Tuple<string, BigInteger>(attestation.authData.redirectUrl, attestation.authData.issuedAt));

            var callData = new Tuple<Address, BigInteger, Byte[], BigInteger, bool, bool, BigInteger>(call.to, call.value, 
                call.data, call.gasLimit, call.delegateCall, call.onlyFallback, (int)call.behaviorOnError);
            
            return ABI.ABI.Pack(
                "acceptImplicitRequest(address,(address,bytes4,bytes32,bytes32,bytes,(string,uint64)),(address,uint256,bytes,uint256,bool,bool,uint256))",
                ParentAddress, attestationData, callData);
        }

        private string GenerateImplicitRequestMagic(Address address, Attestation attestation)
        {
            return SequenceCoder.KeccakHash(
                ByteArrayExtensions.ConcatenateByteArrays(
                    SequenceCoder.KeccakHash("acceptImplicitRequest".ToByteArray()),
                    address.Value.HexStringToByteArray(20),
                    attestation.audienceHash.Data,
                    attestation.issuerHash.Data))
                .ByteArrayToHexStringWithPrefix();
        }
    }
}