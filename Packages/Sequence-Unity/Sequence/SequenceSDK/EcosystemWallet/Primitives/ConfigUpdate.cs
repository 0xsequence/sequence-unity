using System;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    internal class ConfigUpdate : Payload
    {
        public override PayloadType type => PayloadType.ConfigUpdate;
        public string imageHash;

        [Preserve]
        public ConfigUpdate(string imageHash)
        {
            this.imageHash = imageHash;
        }
    }
}