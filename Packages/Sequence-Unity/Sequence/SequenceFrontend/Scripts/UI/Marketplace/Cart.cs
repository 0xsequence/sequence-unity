using System;
using System.Collections.Generic;
using Sequence.Marketplace;
using UnityEngine;

namespace Sequence.Demo
{
    public class Cart
    {
        public CollectibleOrder[] Listings;
        public Dictionary<string, Sprite> CollectibleImagesByOrderId;
        public Dictionary<string, uint> AmountsRequestedByOrderId;
        
        public Cart(CollectibleOrder[] listings, Dictionary<string, Sprite> collectibleImagesByOrderId, Dictionary<string, uint> amountsRequestedByOrderId)
        {
            Listings = listings;
            CollectibleImagesByOrderId = collectibleImagesByOrderId;
            AmountsRequestedByOrderId = amountsRequestedByOrderId;
            
            int listingsLength = listings.Length;
            if (listingsLength == 0)
            {
                throw new ArgumentException(
                    $"Invalid use. {GetType().Name} must be created with a non-empty {typeof(CollectibleOrder[])}");
            }
            
            for (int i = 0; i < listingsLength; i++)
            {
                CollectibleOrder listing = listings[i];
                if (!collectibleImagesByOrderId.ContainsKey(listing.order.orderId))
                {
                    throw new ArgumentException(
                        $"Invalid use. {GetType().Name} must be created with a {typeof(Dictionary<string, Sprite>)} that contains all orderIds as keys in the {typeof(CollectibleOrder[])}");
                }
            }
            
            for (int i = 0; i < listingsLength; i++)
            {
                CollectibleOrder listing = listings[i];
                if (!amountsRequestedByOrderId.ContainsKey(listing.order.orderId))
                {
                    throw new ArgumentException(
                        $"Invalid use. {GetType().Name} must be created with a {typeof(Dictionary<string, uint>)} that contains all orderIds as keys in the {typeof(CollectibleOrder[])}");
                }
            }
        }
        
        public CollectibleOrder GetOrderByOrderId(string orderId)
        {
            foreach (CollectibleOrder listing in Listings)
            {
                if (listing.order.orderId == orderId)
                {
                    return listing;
                }
            }

            return null;
        }
    }
}