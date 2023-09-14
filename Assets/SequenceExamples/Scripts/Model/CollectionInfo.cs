using System.Collections.Generic;
using UnityEngine;

namespace Sequence.Demo
{
    public class CollectionInfo
    {
        public Address ContractAddress;
        public Sprite IconSprite;
        public string Name;
        public Chain Network;

        private static List<CollectionInfo> _collections = new List<CollectionInfo>();

        private CollectionInfo(Address contractAddress, Sprite iconSprite, string name, Chain network)
        {
            ContractAddress = contractAddress;
            IconSprite = iconSprite;
            Name = name;
            Network = network;
        }

        public override bool Equals(object obj)
        {
            if (obj is CollectionInfo info)
            {
                return ContractAddress.Value == info.ContractAddress.Value &&
                       IconSprite == info.IconSprite &&
                       Name == info.Name &&
                       Network == info.Network;
            }

            return false;
        }

        public static CollectionInfo GetCollectionInfo(Address contractAddress, Sprite iconSprite, string name, Chain network)
        {
            CollectionInfo temp = new CollectionInfo(contractAddress, iconSprite, name, network);
            int collectionsLength = _collections.Count;
            for (int i = 0; i < collectionsLength; i++)
            {
                if (temp.Equals(_collections[i]))
                {
                    return _collections[i];
                }
            }
            _collections.Add(temp);
            return temp;
        }

        public static void EvictCollections()
        {
            _collections = new List<CollectionInfo>();
        }
    }
}