using System.Collections.Generic;
using UnityEngine;

namespace Sequence.Demo
{
    public class CollectionInfo : ISearchable
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
                       Name == info.Name &&
                       Network == info.Network;
            }

            return false;
        }

        /// <summary>
        /// Returns the CollectionInfo that matches the provided params or creates a new one if none exists
        /// Note: iconSprite is not used to match with a CollectionInfo. If you provide an iconSprite that diverges from what has already been provided,
        /// which is unlikely for most use cases, then you will overwrite the iconSprite associated with the corresponding CollectionInfo
        /// </summary>
        /// <param name="contractAddress"></param>
        /// <param name="iconSprite"></param>
        /// <param name="name"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        public static CollectionInfo GetCollectionInfo(Address contractAddress, Sprite iconSprite, string name, Chain network)
        {
            CollectionInfo temp = new CollectionInfo(contractAddress, iconSprite, name, network);
            int collectionsLength = _collections.Count;
            for (int i = 0; i < collectionsLength; i++)
            {
                if (temp.Equals(_collections[i]))
                {
                    _collections[i].IconSprite = iconSprite;
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

        public Sprite GetIcon()
        {
            return IconSprite;
        }

        public string GetName()
        {
            return Name;
        }

        public Chain GetNetwork()
        {
            return Network;
        }
    }
}