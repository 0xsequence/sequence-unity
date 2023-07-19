using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sequence.WaaS
{
    [System.Serializable]
    public class PartnerWalletConfig
    {
        public uint id { get; private set; }
        public uint partnerId { get; private set; }
        public string address { get; private set; }
        public string config { get; private set; }

        public PartnerWalletConfig(uint id, uint partnerId, string address, string config)
        {
            this.id = id;
            this.partnerId = partnerId;
            this.address = address;
            this.config = config;
        }
    }
}
