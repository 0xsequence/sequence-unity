using System;
using Newtonsoft.Json;

namespace Sequence.Marketplace
{
    [Serializable]
    public class Domain
    {
        public string name;
        public string version;
        public uint chainId;
        public Chain chain;
        public string verifyingContract;

        [JsonConstructor]
        public Domain(string name, string version, uint chainId, string verifyingContract)
        {
            this.name = name;
            this.version = version;
            this.chainId = chainId;
            this.chain = ChainDictionaries.ChainById[chainId.ToString()];
            this.verifyingContract = verifyingContract;
        }

        public Domain(string name, string version, Chain chain, Address verifyingContract)
        {
            this.name = name;
            this.version = version;
            this.chainId = uint.Parse(ChainDictionaries.ChainIdOf[chain]);
            this.chain = chain;
            this.verifyingContract = verifyingContract;
        }
    }
}