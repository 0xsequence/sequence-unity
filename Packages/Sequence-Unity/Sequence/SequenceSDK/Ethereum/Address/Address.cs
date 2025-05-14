using System;
using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence {
    
    [JsonConverter(typeof(AddressJsonConverter))]
    public class Address {
        public readonly string Value;

        public static readonly Address ZeroAddress = new Address(StringExtensions.ZeroAddress);

        /// <summary>
        /// Caution: this constructor will throw an exception if you supply an invalid address string
        /// </summary>
        /// <param name="value"></param>
        public Address(string value) {
            ValidateIsAddress(value);
            Value = value;
        }

        private void ValidateIsAddress(string value)
        {
            if (!value.IsAddress())
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        public Address(byte[] bytes)
        {
            string value = bytes.ByteArrayToHexStringWithPrefix();
            ValidateIsAddress(value);
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
            return Value.Equals(address.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Value.ToLowerInvariant().GetHashCode();
        }
    }

    public class AddressJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Address address)
            {
                writer.WriteValue(address.Value);
            }
            else
            {
                throw new JsonSerializationException("Expected Address object.");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string addressString = (string)reader.Value;
            
                try
                {
                    return new Address(addressString);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new JsonSerializationException("Invalid address format.");
                }
            }

            throw new JsonSerializationException("Expected a string value.");
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }

    public static class AddressExtensions
    {
        public static bool IsZeroAddress(this Address address)
        {
            return IsZeroAddress(address.Value);
        }

        public static bool IsZeroAddress(this string address)
        {
            string toCheck = address.WithoutHexPrefix();
            toCheck = toCheck.Replace("0", "");
            return toCheck.Length == 0;
        }
    }
}