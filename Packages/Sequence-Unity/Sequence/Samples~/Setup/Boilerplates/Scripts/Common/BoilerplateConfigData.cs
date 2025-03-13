using System;
using UnityEngine.Scripting;

namespace Sequence.Boilerplates
{
    [Preserve]
    [Serializable]
    public class BoilerplateConfigData
    {
        public bool useProjectKeys;
        public bool playerProfile;
        public bool signMessage;
        public string waasConfigKey;
        public string projectAccessKey;
        public string googleClientId;
        public string appleClientId;
        public string chainId;
        public string rewardsApi;
        public string[] collections;
        public PrimarySaleConfig[] primarySales;
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
}
