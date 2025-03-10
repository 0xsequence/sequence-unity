using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class FeeOption
    {
        public uint gasLimit;
        public string to;
        public FeeToken token;
        public string value;

        [UnityEngine.Scripting.Preserve]
        public FeeOption(uint gasLimit, string to, FeeToken token, string value)
        {
            this.gasLimit = gasLimit;
            this.to = to;
            this.token = token;
            this.value = value;
        }

        public Transaction CreateTransaction()
        {
            switch (token.type)
            {
                case FeeTokenType.unknown:
                    return new RawTransaction(to, value);
                case FeeTokenType.erc20Token:
                    return new SendERC20(token.contractAddress, to, value);
                case FeeTokenType.erc1155Token:
                    return new SendERC1155(token.contractAddress, to, new SendERC1155Values[]
                    {
                        new SendERC1155Values(token.tokenID, value)
                    });
                default:
                    throw new ArgumentException("Invalid FeeTokenType. Given: " + token.type);
            }
        }

        public override string ToString()
        {
            return $"gasLimit: {gasLimit}, to: {to}, token: [{token}], value: {value}";
        }
    }
}