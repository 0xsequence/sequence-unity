using System;
using UnityEngine.Scripting;

namespace Sequence
{
    [Serializable]
    public class TokenPrice
    {
        public Token token;
        public Price price;
        public Price price24hChange;
        public Price floorPrice;
        public Price buyPrice;
        public Price sellPrice;
        public DateTime updatedAt;
        
        [Preserve]
        public TokenPrice(Token token, Price price, Price price24hChange, Price floorPrice, Price buyPrice, Price sellPrice, DateTime updatedAt)
        {
            this.token = token;
            this.price = price;
            this.price24hChange = price24hChange;
            this.floorPrice = floorPrice;
            this.buyPrice = buyPrice;
            this.sellPrice = sellPrice;
            this.updatedAt = updatedAt;
        }
    }
}