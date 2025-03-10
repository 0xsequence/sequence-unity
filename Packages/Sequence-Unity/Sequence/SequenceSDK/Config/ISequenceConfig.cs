namespace Sequence.Config
{
    public interface ISequenceConfig
    {
        string UrlScheme { get; }
        string GoogleClientId { get; }
        string DiscordClientId { get; }
        string FacebookClientId { get; }
        string AppleClientId { get; }
        string DiscordClientIdIOS { get; }
        string FacebookClientIdIOS { get; }
        string AppleClientIdIOS { get; }
        string WaaSConfigKey { get; }
        string WaaSVersion { get; set; }
        string BuilderAPIKey { get; }
        bool EnableMultipleAccountsPerEmail { get; }
        bool StoreSessionPrivateKeyInSecureStorage { get; set; }
        bool EditorStoreSessionPrivateKeyInSecureStorage { get; }
        bool StoreSessionKey();
    }
}