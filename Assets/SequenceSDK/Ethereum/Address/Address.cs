using Sequence.ABI;

namespace Sequence {
    public class Address {
        public readonly string Value;

        public Address(string value) {
            Value = value;
        }

        public static implicit operator Address(string value) {
            return new Address(value);
        }

        public static implicit operator string(Address address)
        {
            return address.Value;
        }
    }
}