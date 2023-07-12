using System;
using Sequence.Extensions;

namespace Sequence {
    public class Address {
        public readonly string Value;

        /// <summary>
        /// Caution: this constructor will throw an exception if you supply an invalid address string
        /// </summary>
        /// <param name="value"></param>
        public Address(string value) {
            if (!value.IsAddress())
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            Value = value;
        }

        public static implicit operator string(Address address)
        {
            return address.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}