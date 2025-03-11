namespace Sequence.Config
{
    public class SequenceConfigBase
    {
#if UNITY_2017_1_OR_NEWER
        public string UrlScheme;
        public string GoogleClientId;
        public string DiscordClientId;
        public string FacebookClientId;
        public string AppleClientId;
        public string GoogleClientIdIOS;
        public string DiscordClientIdIOS;
        public string FacebookClientIdIOS;
        public string AppleClientIdIOS;
        public string WaaSConfigKey;
        public string WaaSVersion;
        public string BuilderAPIKey;
        public bool EnableMultipleAccountsPerEmail;
        public bool StoreSessionPrivateKeyInSecureStorage;
        public bool EditorStoreSessionPrivateKeyInSecureStorage;
#else
        public string WaaSVersion;
        public string BuilderAPIKey;
        public string WaaSConfigKey;
#endif

        public bool StoreSessionKey()
        {
#if UNITY_EDITOR
            return EditorStoreSessionPrivateKeyInSecureStorage &&
                   NUnit.Framework.TestContext.CurrentTestExecutionContext?.ExecutionStatus != 
                   NUnit.Framework.Internal.TestExecutionStatus.Running;
#elif UNITY_2017_1_OR_NEWER
            return StoreSessionPrivateKeyInSecureStorage;
#else
            return false;
#endif
        }
    }
}