using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class CreateReq
    {
        public string tokenId;
        public string quantity;
        public string expiry;
        public string currencyAddress;
        public string pricePerToken;

        [Preserve]
        public CreateReq(string tokenId, string quantity, string expiry, string currencyAddress, string pricePerToken)
        {
            this.tokenId = tokenId;
            this.quantity = quantity;
            this.expiry = expiry;
            this.currencyAddress = currencyAddress;
            this.pricePerToken = pricePerToken;
        }

        public override string ToString()
        {
            return $"CreateReq(tokenId: {tokenId}, quantity: {quantity}, expiry: {expiry}, currencyAddress: {currencyAddress}, pricePerToken: {pricePerToken})";
        }
    }
}