using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.Pay.Transak
{
    [Serializable]
    public class TransakNftData
    {
        public string imageURL;
        public string nftName;
        public Address collectionAddress;
        public string[] tokenID;
        public decimal[] price;
        public ulong quantity;
        public NFTType nftType;

        [Preserve]
        public TransakNftData(string imageURL, string nftName, Address collectionAddress, string[] tokenID, decimal[] price, ulong quantity, NFTType nftType = NFTType.ERC721)
        {
            this.imageURL = imageURL;
            this.nftName = nftName;
            this.collectionAddress = collectionAddress;
            this.tokenID = tokenID;
            this.price = price;
            this.quantity = quantity;
            this.nftType = nftType;
        }
    }

    [JsonConverter(typeof(EnumConverter<NFTType>))]
    public enum NFTType
    {
        ERC1155,
        ERC721
    }
}