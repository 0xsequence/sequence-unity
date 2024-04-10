using System;
using System.Numerics;
using Sequence;
using Sequence.Contracts;
using Sequence.WaaS;
using SequenceSDK.WaaS;

namespace Sequence.Relayer
{
    public class QueuedTokenTransaction : IQueueableTransaction
    {
        public enum TransactionType
        {
            MINT,
            TRANSFER,
            BURN,
            OTHER
        }

        public enum TokenType
        {
            ERC20,
            ERC721,
            ERC1155,
            NONE
        }

        public TransactionType Type;
        private TokenType _tokenType;
        public string ContractAddress;
        public string TokenId;
        public Address ToAddress;
        public BigInteger Amount;
        public Address FromAddress;
        
        public QueuedTokenTransaction(TransactionType type, TokenType tokenType, string contractAddress, string tokenId, BigInteger amount, Address fromAddress, Address toAddress = null)
        {
            Type = type;
            _tokenType = tokenType;
            ContractAddress = contractAddress;
            TokenId = tokenId;
            Amount = amount;
            ToAddress = toAddress;
        }

        public Transaction BuildTransaction()
        {
            switch (_tokenType)
            {
                case TokenType.ERC20:
                    ERC20 erc20 = new ERC20(ContractAddress);
                    switch (Type)
                    {
                        case TransactionType.MINT:
                            return new RawTransaction(erc20.Mint(ToAddress, Amount));
                        case TransactionType.TRANSFER:
                            return new RawTransaction(erc20.Transfer(ToAddress, Amount));
                        case TransactionType.BURN:
                            return new RawTransaction(erc20.Burn(Amount)); 
                        default:
                            throw new ArgumentException("Unsupported transaction type");
                    }
                case TokenType.ERC721:
                    ERC721 erc721 = new ERC721(ContractAddress);
                    BigInteger tokenId = BigInteger.Parse(TokenId);
                    switch (Type)
                    {
                        case TransactionType.MINT:
                            return new RawTransaction(erc721.SafeMint(ToAddress, tokenId));
                        case TransactionType.TRANSFER:
                            return new RawTransaction(erc721.SafeTransferFrom(FromAddress, ToAddress, tokenId));
                        case TransactionType.BURN:
                            return new RawTransaction(erc721.Burn(tokenId)); 
                        default:
                            throw new ArgumentException("Unsupported transaction type");
                    }
                case TokenType.ERC1155:
                    ERC1155 erc1155 = new ERC1155(ContractAddress);
                    tokenId = BigInteger.Parse(TokenId);
                    switch (Type)
                    {
                        case TransactionType.MINT:
                            return new RawTransaction(erc1155.Mint(ToAddress, tokenId, Amount));
                        case TransactionType.TRANSFER:
                            return new RawTransaction(erc1155.SafeTransferFrom(FromAddress, ToAddress, tokenId, Amount));
                        case TransactionType.BURN:
                            return new RawTransaction(erc1155.Burn(tokenId, Amount)); 
                        default:
                            throw new ArgumentException("Unsupported transaction type");
                    }
                default:
                    throw new ArgumentException("Unsupported token type");
            }
        }
        
        public override string ToString()
        {
            return $"Type: {Type}, TokenType: {_tokenType}, ContractAddress: {ContractAddress}, TokenId: {TokenId}, ToAddress: {ToAddress}, Amount: {Amount}, FromAddress: {FromAddress}";
        }
    }
}