using System.Numerics;

namespace Sequence.Relayer
{
    public class FeeOption
    {
        public FeeToken token;
        public string to;
        public string value;
        public int gasLimit;
    }

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