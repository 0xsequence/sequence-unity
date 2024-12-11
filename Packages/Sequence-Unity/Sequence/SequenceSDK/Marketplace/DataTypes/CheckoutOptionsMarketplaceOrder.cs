using System;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class CheckoutOptionsMarketplaceOrder
    {
        public Address contractAddress;
        public string orderId;
        public MarketplaceKind marketplace;

        [Preserve]
        public CheckoutOptionsMarketplaceOrder(Address contractAddress, string orderId, MarketplaceKind marketplace)
        {
            this.contractAddress = contractAddress;
            this.orderId = orderId;
            this.marketplace = marketplace;
        }
    }

    public static class CheckoutOptionsMarketplaceOrderExtensions
    {
        public static string AsString(this CheckoutOptionsMarketplaceOrder[] orders)
        {
            if (orders == null)
            {
                return "";
            }
            int length = orders.Length;
            if (length == 0)
            {
                return "[]";
            }

            StringBuilder result = new StringBuilder("[");
            for (int i = 0; i < length - 1; i++)
            {
                result.Append($"contractAddress: {orders[i].contractAddress}, orderId: {orders[i].orderId}, marketplace: {orders[i].marketplace}");
                result.Append(" | ");
            }
            result.Append($"contractAddress: {orders[length - 1].contractAddress}, orderId: {orders[length - 1].orderId}, marketplace: {orders[length - 1].marketplace}");
            result.Append("]");
            return result.ToString();
        }
    }
}