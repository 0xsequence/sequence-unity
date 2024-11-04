using System;
using System.Numerics;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Preserve]
    [Serializable]
    public class PropertyFilter
    {
        public string name;
        public string type;
        public BigInteger min;
        public BigInteger max;
        public object[] values;
        
        public PropertyFilter(string name, PropertyType type, BigInteger min = default, BigInteger max = default, object[] values = null)
        {
            this.name = name;
            this.type = type.AsString();
            this.min = min;
            this.max = max;
            this.values = values;
        }

        [Preserve]
        [JsonConstructor]
        public PropertyFilter(string name, string type, BigInteger min, BigInteger max, object[] values)
        {
            this.name = name;
            this.type = type;
            this.min = min;
            this.max = max;
            this.values = values;
        }
    }
}