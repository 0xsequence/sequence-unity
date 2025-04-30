using System;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    internal class Digest : Payload
    {
        public override PayloadType type => PayloadType.Digest;
        public string digest;

        [Preserve]
        public Digest(string digest)
        {
            this.digest = digest;
        }
    }
}