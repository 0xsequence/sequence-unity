using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sequence.WaaS
{
    [System.Serializable]
    public class PartnerWallet
    {
        public uint id { get; private set; }
        public uint partnerId { get; private set; }
        public uint walletIndex { get; private set; }
        public string walletAddress { get; private set; }

        public PartnerWallet(uint id, uint partnerId, uint walletIndex, string walletAddress)
        {
            this.id = id;
            this.partnerId = partnerId;
            this.walletIndex = walletIndex;
            this.walletAddress = walletAddress;
        }
    }
}
