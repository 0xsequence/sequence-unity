using System.Numerics;

namespace Sequence.Marketplace
{
    public class PropertyFilter
    {
        public string name;
        public PropertyType type;
        public BigInteger min;
        public BigInteger max;
        public object[] values;
    }
}