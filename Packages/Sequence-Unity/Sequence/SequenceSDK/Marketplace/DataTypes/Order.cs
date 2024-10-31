using System;
using System.Numerics;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace Sequence.Marketplace
{
    [Serializable]
    public class Order
    {
        public BigInteger id;
        public BigInteger collectionId;
        public BigInteger collectibleId;
        public string orderId;
        public MarketplaceKind marketplace;
        public SourceKind source;
        public OrderSide side;
        public OrderStatus status;
        public BigInteger chainId;
        public string collectionContractAddress;
        public string tokenId;
        public string createdBy;
        public string priceAmount;
        public string priceAmountFormatted;
        public string priceAmountNet;
        public string priceAmountNetFormatted;
        public string priceCurrencyAddress;
        public BigInteger priceDecimals;
        public float priceUSD;
        public string quantityInitial;
        public string quantityInitialFormatted;
        public string quantityRemaining;
        public string quantityRemainingFormatted;
        public string quantityAvailable;
        public string quantityAvailableFormatted;
        public BigInteger quantityDecimals;
        public BigInteger feeBps;
        public FeeBreakdown[] feeBreakdown;
        public string validFrom;
        public string validUntil;
        public string orderCreatedAt;
        public string orderUpdatedAt;
        public string createdAt;
        public string updatedAt;
        public string deletedAt;

        [Preserve]
        public Order(BigInteger id, BigInteger collectionId, BigInteger collectibleId, string orderId, MarketplaceKind marketplace, SourceKind source, OrderSide side, OrderStatus status, BigInteger chainId, string collectionContractAddress, string tokenId, string createdBy, string priceAmount, string priceAmountFormatted, string priceAmountNet, string priceAmountNetFormatted, string priceCurrencyAddress, BigInteger priceDecimals, float priceUsd, string quantityInitial, string quantityInitialFormatted, string quantityRemaining, string quantityRemainingFormatted, string quantityAvailable, string quantityAvailableFormatted, BigInteger quantityDecimals, BigInteger feeBps, FeeBreakdown[] feeBreakdown, string validFrom, string validUntil, string orderCreatedAt, string orderUpdatedAt, string createdAt, string updatedAt, string deletedAt)
        {
            this.id = id;
            this.collectionId = collectionId;
            this.collectibleId = collectibleId;
            this.orderId = orderId;
            this.marketplace = marketplace;
            this.source = source;
            this.side = side;
            this.status = status;
            this.chainId = chainId;
            this.collectionContractAddress = collectionContractAddress;
            this.tokenId = tokenId;
            this.createdBy = createdBy;
            this.priceAmount = priceAmount;
            this.priceAmountFormatted = priceAmountFormatted;
            this.priceAmountNet = priceAmountNet;
            this.priceAmountNetFormatted = priceAmountNetFormatted;
            this.priceCurrencyAddress = priceCurrencyAddress;
            this.priceDecimals = priceDecimals;
            this.priceUSD = priceUsd;
            this.quantityInitial = quantityInitial;
            this.quantityInitialFormatted = quantityInitialFormatted;
            this.quantityRemaining = quantityRemaining;
            this.quantityRemainingFormatted = quantityRemainingFormatted;
            this.quantityAvailable = quantityAvailable;
            this.quantityAvailableFormatted = quantityAvailableFormatted;
            this.quantityDecimals = quantityDecimals;
            this.feeBps = feeBps;
            this.feeBreakdown = feeBreakdown;
            this.validFrom = validFrom;
            this.validUntil = validUntil;
            this.orderCreatedAt = orderCreatedAt;
            this.orderUpdatedAt = orderUpdatedAt;
            this.createdAt = createdAt;
            this.updatedAt = updatedAt;
            this.deletedAt = deletedAt;
        }
    }
}