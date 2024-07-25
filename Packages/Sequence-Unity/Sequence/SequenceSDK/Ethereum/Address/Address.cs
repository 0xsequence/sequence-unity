using System;
using Sequence.Utils;

namespace Sequence {
    public class Address {
        public readonly string Value;

        public static readonly Address ZeroAddress = new Address(StringExtensions.ZeroAddress);

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

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Address))
            {
                return false;
            }

            Address address = (Address)obj;
            return this.Value == address.Value;
        }
    }
}