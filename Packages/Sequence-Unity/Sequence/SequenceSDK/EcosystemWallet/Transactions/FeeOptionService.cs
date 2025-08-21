using System.Numerics;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Relayer;
using Sequence.Utils;

namespace Sequence.EcosystemWallet
{
    internal class FeeOptionService
    {
        private static readonly Address ValueForwarder = new ("0xABAAd93EeE2a569cF0632f39B10A9f5D734777ca");
        
        private readonly FeeOption _feeOption;
        
        public FeeOptionService(FeeOption feeOption)
        {
            _feeOption = feeOption;
        }

        public Call BuildCallForNativeTokenOption()
        {
            var encodedFeeOptionData = ABI.ABI.Pack("forwardValue(address,uint256)",
                _feeOption.to, BigInteger.Parse(_feeOption.value)).HexStringToByteArray();
            
            return new Call(
                ValueForwarder,
                BigInteger.Parse(_feeOption.value),
                encodedFeeOptionData,
                _feeOption.gasLimit,
                false,
                false,
                BehaviourOnError.revert);
        }
        
        public Call BuildCallForCustomTokenOption()
        {
            var encodedFeeOptionData = ABI.ABI.Pack("transfer(address,uint256)",
                _feeOption.to, BigInteger.Parse(_feeOption.value)).HexStringToByteArray();
            
            return new Call(
                _feeOption.token.contractAddress,
                0,
                encodedFeeOptionData,
                _feeOption.gasLimit,
                false,
                false,
                BehaviourOnError.revert);
        }
    }
}