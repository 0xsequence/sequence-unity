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

        public CollectionInfo(Address contractAddress, Sprite iconSprite, string name, Chain network)
        {
            ContractAddress = contractAddress;
            IconSprite = iconSprite;
            Name = name;
            Network = network;
        }
    }
}