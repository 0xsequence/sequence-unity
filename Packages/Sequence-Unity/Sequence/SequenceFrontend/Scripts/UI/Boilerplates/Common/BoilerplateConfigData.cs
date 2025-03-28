using System;
using UnityEngine.Scripting;

namespace Sequence.Boilerplates
{
    [Preserve]
    [Serializable]
    public class BoilerplateConfigData
    {
        public bool playerProfile;
        public bool signMessage;
        public string chainId;
        public string rewardsApi;
        public string[] collections;
        public PrimarySaleConfig[] primarySales;
        public SecondarySaleConfig secondarySale;
        public bool checkout;
    }
    
    [Preserve]
    [Serializable]
    public class PrimarySaleConfig
    {
        public string name;
        public string collectionAddress;
        public string saleAddress;
        public int[] itemsForSale;
    }

    [Preserve]
    [Serializable]
    public class SecondarySaleConfig
    {
        public string collectionAddress;
        public Chain chain;
    }
}
