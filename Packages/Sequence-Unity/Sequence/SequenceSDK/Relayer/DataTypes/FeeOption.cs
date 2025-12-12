using System.Numerics;
using UnityEngine.Scripting;

namespace Sequence.Relayer
{
    [Preserve]
    public class FeeOption
    {
        public FeeToken token;
        public string to;
        public string value;
        public int gasLimit;
    }

    [Preserve]
    public class FeeToken
    {
        public BigInteger chainId;
        public Address contractAddress;
        public int decimals;
        public string logoURL;
        public string name;
        public string symbol;
        public string tokenID;
        public string type;
    }
}