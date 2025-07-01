using System;
using System.Linq;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SessionBranch : IBranch
    {
        public ITopology[] Children { get; set; }

        /// <summary>
        /// Create a session branch of two children.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public SessionBranch(SessionsTopology left, SessionsTopology right)
        {
            Children = new ITopology[] { left, right };
        }

        /// <summary>
        /// Create a session branch with more than two children.
        /// </summary>
        /// <param name="children"></param>
        /// <exception cref="Exception"></exception>
        public SessionBranch(ITopology[] children)
        {
            if (children.Length < 2)
                throw new Exception("Session Branch has less than 2 children.");
            
            Children = children;
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