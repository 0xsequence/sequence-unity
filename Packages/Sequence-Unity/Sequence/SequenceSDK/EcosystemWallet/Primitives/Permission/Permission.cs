using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    public class Permission
    {
        public const int MAX_PERMISSIONS_COUNT = (1 << 7) - 1;
        public const int MAX_RULES_COUNT = (1 << 8) - 1;
        
        public Address target;
        public ParameterRule[] rules;

        public object ToJson()
        {
            return new
            {
                target = target.Value,
                rules = rules.Select(rule => rule.ToJson()).ToArray(),
            };
        }

        public byte[] Encode()
        {
            if (rules.Length > MAX_RULES_COUNT) {
                throw new Exception("Too many rules");
            }

            List<byte> result = new();
            result.AddRange(target.ToString().HexStringToByteArray().PadLeft(20));
            result.Add((byte)rules.Length);

            foreach (var rule in rules) {
                result.AddRange(rule.Encode());
            }

            return result.ToArray();
        }

        public static Permission FromJson(string json)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            return new()
            {
                target = new Address((string)data["target"]),
                rules = JsonConvert.DeserializeObject<List<object>>((string)data["rules"])
                    .Select(r => ParameterRule.FromJson(r.ToString()))
                    .ToArray(),
            };
        }

        public static (Permission Permission, int Consumed) Decode(byte[] data, int offset)
        {
            if (data.Length < offset + 21)
                throw new Exception("Data too short for permission");

            var target = new Address(data.AsSpan(offset, 20).ToArray());
            offset += 20;

            int rulesLength = data[offset];
            offset += 1;

            var rules = new ParameterRule[rulesLength];
            const int ruleSize = 97;
            
            for (var i = 0; i < rulesLength; i++) {
                if (data.Length < offset + ruleSize)
                    throw new Exception("Data too short for parameter rule");

                var ruleBytes = data.AsSpan(offset, ruleSize).ToArray();
                var rule = ParameterRule.Decode(ruleBytes);
                rules[i] = rule;
                offset += ruleSize;
            }

            return (new Permission {
                target = target,
                rules = rules
            }, 20 + 1 + ruleSize * rulesLength);
        }
    }
}