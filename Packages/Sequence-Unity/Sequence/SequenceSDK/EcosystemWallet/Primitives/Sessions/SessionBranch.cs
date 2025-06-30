using System.Linq;
using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SessionBranch
    {
        public SessionsTopology[] Children;

        public SessionBranch(SessionsTopology left, SessionsTopology right)
        {
            Children = new[] { left, right };
        }

        public SessionBranch(SessionsTopology[] children)
        {
            Children = children;
        }

        public object ToJson()
        {
            return Children.Select(child => child.ToJson()).ToArray();
        }

        public byte[] Encode()
        {
            var encodings = Children.Select(child => child.Encode()).ToArray();
            return ByteArrayExtensions.ConcatenateByteArrays(encodings);
        }

        public SessionsTopology ToTopology()
        {
            return new SessionsTopology(this);
        }
    }
}