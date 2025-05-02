using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GetPrimarySaleCheckoutOptionsArgs
    {
        public string wallet;
        public string contractAddress;
        public string collectionAddress;
        public CheckoutOptionsItem[] items;

        [Preserve]
        public GetPrimarySaleCheckoutOptionsArgs(string wallet, string contractAddress, string collectionAddress, CheckoutOptionsItem[] items)
        {
            this.wallet = wallet;
            this.contractAddress = contractAddress;
            this.collectionAddress = collectionAddress;
            this.items = items;
        }

        public GetPrimarySaleCheckoutOptionsArgs(Address wallet, Address contract, Address collection,
            Dictionary<string, BigInteger> amounts)
        {
            if (amounts == null)
            {
                throw new ArgumentException("Must provide at least one tokenId and amount");
            }
            
            int count = amounts.Count;
            if (count == 0)
            {
                throw new ArgumentException("Must provide at least one tokenId and amount");
            }
            
            CheckoutOptionsItem[] items = new CheckoutOptionsItem[count];
            int i = 0;
            foreach (var key in amounts.Keys)
            {
                if (!BigInteger.TryParse(key, out _))
                {
                    throw new ArgumentException($"TokenId is invalid, given: {key}");
                }

                if (amounts[key] <= 0)
                {
                    throw new ArgumentException($"Amount must be larger than 0, given: {amounts[key]}");
                }
                
                items[i] = new CheckoutOptionsItem(key, amounts[key].ToString());
                i++;
            }
            
            this.wallet = wallet.ToString();
            this.contractAddress = contract.ToString();
            this.collectionAddress = collection.ToString();
            this.items = items;
        }
        
        public GetPrimarySaleCheckoutOptionsArgs(Address wallet, Address contract, Address collection,
            string tokenId, BigInteger amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException($"Amount must be larger than 0, given: {amount}");
            }
            
            if (!BigInteger.TryParse(tokenId, out _))
            {
                throw new ArgumentException($"TokenId is invalid, given: {tokenId}");
            }
            
            CheckoutOptionsItem[] items = {new CheckoutOptionsItem(tokenId, amount.ToString())};
            this.wallet = wallet.ToString();
            this.contractAddress = contract.ToString();
            this.collectionAddress = collection.ToString();
            this.items = items;
        }
    }
    
    [Serializable]
    internal class CheckoutOptionsItem
    {
        public string tokenId;
        public string quantity;

        [Preserve]
        public CheckoutOptionsItem(string tokenId, string quantity)
        {
            this.tokenId = tokenId;
            this.quantity = quantity;
        }
    }
}