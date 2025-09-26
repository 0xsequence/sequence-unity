using Newtonsoft.Json;
using Sequence.Utils.SecureStorage;

namespace Sequence.EcosystemWallet
{
    public class GuardStorage
    {
        private readonly ISecureStorage _storage = SecureStorageFactory.CreateSecureStorage();

        public GuardConfig GetConfig(Address address)
        {
            var json = _storage.RetrieveString(BuildStorageKey(address));
            return string.IsNullOrEmpty(json) ? null : 
                JsonConvert.DeserializeObject<GuardConfig>(json);
        }
        
        public void SaveConfig(Address address, GuardConfig config)
        {
            var json = JsonConvert.SerializeObject(config);
            _storage.StoreString(BuildStorageKey(address), json);
        }

        private string BuildStorageKey(Address address)
        {
            return $"sequence-guard-config-{address}";
        }
    }
}