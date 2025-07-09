using Sequence.EcosystemWallet.Primitives;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Authentication
{
    [Preserve]
    public class AuthPayload
    {
        public Address sessionAddress;
        public object permissions;
        public string preferredLoginMethod;
        public string email;
    }
}