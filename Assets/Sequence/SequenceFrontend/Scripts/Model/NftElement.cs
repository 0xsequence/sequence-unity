using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace Sequence.Demo
{
    public class NftElement : ICurrencyRepository
    {
        public CollectionInfo Collection;
        public Sprite TokenIconSprite;
        public string TokenName;
        public BigInteger TokenNumber;
        public uint Balance;
        public float EthValue;
        public ICurrencyConverter CurrencyConverter;

        public NftElement(CollectionInfo collection, Sprite tokenIconSprite, string tokenName, BigInteger tokenNumber, uint balance, float ethValue, ICurrencyConverter currencyConverter)
        {
            Collection = collection;
            TokenIconSprite = tokenIconSprite;
            TokenName = tokenName;
            TokenNumber = tokenNumber;
            Balance = balance;
            EthValue = ethValue;
            CurrencyConverter = currencyConverter;
        }

        public NftElement(Address contractAddress, Sprite tokenIconSprite, string tokenName,
            Sprite collectionIconSprite, string collectionName, BigInteger tokenNumber, Chain network, uint balance,
            float ethValue, ICurrencyConverter currencyConverter)
        {
            CollectionInfo collection = CollectionInfo.GetCollectionInfo(contractAddress, collectionIconSprite, collectionName, network);
            
            Collection = collection;
            TokenIconSprite = tokenIconSprite;
            TokenName = tokenName;
            TokenNumber = tokenNumber;
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

        public string GetSymbol()
        {
            return "ETH";
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

        public Address GetContractAddress()
        {
            return Collection.ContractAddress;
        }
        
        public static uint CalculateTotalNftsOwned(List<NftElement> nfts)
        {
            int count = nfts.Count;
            uint owned = 0;
            for (int i = 0; i < count; i++)
            {
                owned += nfts[i].Balance;
            }
            return owned;
        }
    }
}