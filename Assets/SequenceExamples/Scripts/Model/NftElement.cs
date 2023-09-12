using UnityEngine;

namespace Sequence.Demo
{
    public class NftElement : ICurrencyRepository
    {
        public Address ContractAddress;
        public Sprite TokenIconSprite;
        public string TokenName;
        public Sprite CollectionIconSprite;
        public string CollectionName;
        public uint TokenNumber;
        public Chain Network;
        public uint Balance;
        public float EthValue;
        public ICurrencyConverter CurrencyConverter;

        public NftElement(Address contractAddress, Sprite tokenIconSprite, string tokenName, Sprite collectionIconSprite, string collectionName, uint tokenNumber, Chain network, uint balance, float ethValue, ICurrencyConverter currencyConverter)
        {
            ContractAddress = contractAddress;
            TokenIconSprite = tokenIconSprite;
            TokenName = tokenName;
            CollectionIconSprite = collectionIconSprite;
            CollectionName = collectionName;
            TokenNumber = tokenNumber;
            Network = network;
            Balance = balance;
            EthValue = ethValue;
            CurrencyConverter = currencyConverter;
        }

        public uint GetAmount()
        {
            return Balance;
        }

        public void SetAmount(uint value)
        {
            Balance = value;
        }

        public string GetAddress()
        {
            return ContractAddress;
        }

        public string GetSymbol()
        {
            return "";
        }

        public float GetPreviousCurrencyValue()
        {
            throw new System.NotSupportedException($"A {nameof(NftElement)} has no need to keep track of previous currency values.");
        }

        public void SetPreviousCurrencyValue(float value)
        {
            // Do nothing
        }

        public Currency GetCurrency()
        {
            return CurrencyConverter.ConvertToCurrency(EthValue, "ETH");
        }
    }
}