using System.Collections.Generic;
using Newtonsoft.Json;
using Sequence.Utils.SecureStorage;

namespace Sequence.EcosystemWallet
{
    public class GuardStorage
    {
        private static readonly Dictionary<Address, GuardConfig> Configs = new();
        
        private readonly ISecureStorage _storage = SecureStorageFactory.CreateSecureStorage();

        public GuardConfig GetConfig(Address address)
        {
            if (Configs.TryGetValue(address, out var memorizedConfig))
                return memorizedConfig;
            
            var json = _storage.RetrieveString(BuildStorageKey(address));
            var config = string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<GuardConfig>(json);
            if (config != null)
                Configs[address] = config;
            
            return config;
        }
        
        public void SaveConfig(Address address, GuardConfig config)
        {
            var json = JsonConvert.SerializeObject(config);
            _storage.StoreString(BuildStorageKey(address), json);
            Configs[address] = config;
        }

        private string BuildStorageKey(Address address)
        {
            return $"sequence-guard-config-{address}";
        }
    }
}