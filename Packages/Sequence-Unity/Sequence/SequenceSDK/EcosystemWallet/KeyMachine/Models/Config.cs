using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.KeyMachine.Models
{
    [Preserve]
    internal struct ConfigArgs
    {
        public string imageHash;

        public ConfigArgs(string imageHash)
        {
            this.imageHash = imageHash;
        }
    }

    [Preserve]
    internal struct ConfigReturn
    {
        public int version;
        public ConfigContext config;
    }

    [Preserve]
    internal struct ConfigContext
    {
        public string checkpoint;
        public int threshold;
        public object tree;
    }
}