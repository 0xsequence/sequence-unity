using System;

namespace Sequence
{
    using System.Numerics;

    [System.Serializable]
    public class TokenBalance
    {
        public int id;
        public string contractAddress;
        public ContractType contractType;
        public string accountAddress;
        public BigInteger tokenID;
        public BigInteger balance;
        public string blockHash;
        public BigInteger blockNumber;
        public BigInteger updateID;
        public BigInteger chainId;
        public ContractInfo contractInfo;
        public TokenMetadata tokenMetadata;
        
        public bool IsNft()
        {
            return contractType == ContractType.ERC721 || contractType == ContractType.ERC1155;
        }

        public bool IsToken()
        {
            return contractType == ContractType.ERC20;
        }
        
        public override string ToString()
        {
            return $"TokenBalance: id: {id}, contractAddress: {contractAddress}, contractType: {contractType}, accountAddress: {accountAddress}, tokenID: {tokenID}, balance: {balance}, blockHash: {blockHash}, blockNumber: {blockNumber}, updateID: {updateID}, chainId: {chainId}, contractInfo: {contractInfo}, tokenMetadata: {tokenMetadata}";
        }
    }

    public static class TokenBalanceExtensions
    {
        public static bool ContainsToken(this TokenBalance[] balances, string tokenAddress)
        {
            return balances.TokenIndex(tokenAddress) != -1;
        }
        
        public static int TokenIndex(this TokenBalance[] balances, string tokenAddress)
        {
            int length = balances.Length;
            for (int i = 0; i < length; i++)
            {
                if (String.Equals(balances[i].contractAddress, tokenAddress, StringComparison.CurrentCultureIgnoreCase))
                {
                    return i;
                }
            }
            
            return -1;
        }
    }
}