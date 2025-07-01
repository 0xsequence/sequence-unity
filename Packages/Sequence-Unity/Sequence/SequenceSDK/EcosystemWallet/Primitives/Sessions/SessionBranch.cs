using System;
using System.Linq;
using Newtonsoft.Json;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SessionBranch : IBranch
    {
        public ITopology[] Children { get; set; }

        public SessionBranch(SessionsTopology left, SessionsTopology right)
        {
            Children = new ITopology[] { left, right };
        }

        public SessionBranch(ITopology[] children)
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
            var encoded = ByteArrayExtensions.ConcatenateByteArrays(encodings);
            var encodedSize = encoded.Length.MinBytesFor();
            if (encodedSize > 15)
                throw new Exception("Session Branch is too large.");
            
            var flag = (SessionsTopology.FlagBranch << 4) | encodedSize;
            
            return ByteArrayExtensions.ConcatenateByteArrays(
                flag.ByteArrayFromNumber(flag.MinBytesFor()), 
                encoded.Length.ByteArrayFromNumber(encodedSize), 
                encoded);
        }

        public SessionsTopology ToTopology()
        {
            return new SessionsTopology(this);
        }
    }
}