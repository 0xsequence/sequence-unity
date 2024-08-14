using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sequence.Config
{
    [CreateAssetMenu(fileName = "SequenceConfig", menuName = "Sequence/SequenceConfig", order = 1)]
    public class SequenceConfig : ScriptableObject
    {
        [Header("Social Sign In Configuration - Standalone & Web Platforms")]
        public string UrlScheme;
        public string GoogleClientId;
        public string DiscordClientId;
        public string FacebookClientId;
        public string AppleClientId;
        
        [Header("Social Sign In Configuration - iOS")]
        public string GoogleClientIdIOS;
        public string DiscordClientIdIOS;
        public string FacebookClientIdIOS;
        public string AppleClientIdIOS;
        
        [Header("Social Sign In Configuration - Android")]
        public string GoogleClientIdAndroid;
        public string DiscordClientIdAndroid;
        public string FacebookClientIAndroid;
        public string AppleClientIdAndroid;

        [Header("WaaS Configuration")]
        public string WaaSConfigKey;
        [FormerlySerializedAs("EnableAccountOverride")] public bool EnableMultipleAccountsPerEmail = false;
        public string WaaSVersion { get; private set; }

        [Header("Sequence SDK Configuration")] 
        public string BuilderAPIKey;
        public bool StoreSessionPrivateKeyInSecureStorage = false;
        public bool EditorStoreSessionPrivateKeyInSecureStorage = false;
        
        private static SequenceConfig _config;

        public static SequenceConfig GetConfig()
        {
            if (_config == null)
            {
                _config = Resources.Load<SequenceConfig>("SequenceConfig");
                TextAsset versionFile = Resources.Load<TextAsset>("sequence-unity-version");
                if (versionFile != null)
                {
                    _config.WaaSVersion = $"1 (Unity {versionFile.text})";
                }
                else
                {
                    _config.WaaSVersion = $"1 (Unity {PackageVersionReader.GetVersion()})";
                }
            }

            if (_config == null)
            {
                throw new Exception("SequenceConfig not found. Make sure to create and configure it and place it at the root of your Resources folder. Create it from the top bar with Assets > Create > Sequence > SequenceConfig");
            }

            return _config;
        }

        public static Exception MissingConfigError(string valueName)
        {
            return new Exception($"{valueName} is not set. Please set it in SequenceConfig asset in your Resources folder.");
        }

        public static ConfigJwt GetConfigJwt()
        {
            string configKey = _config.WaaSConfigKey;
            if (string.IsNullOrWhiteSpace(configKey))
            {
                throw SequenceConfig.MissingConfigError("WaaS Config Key");
            }

            return JwtHelper.GetConfigJwt(configKey);
        }

        public bool StoreSessionKey()
        {
#if UNITY_EDITOR
            return EditorStoreSessionPrivateKeyInSecureStorage;
#else
            return StoreSessionPrivateKeyInSecureStorage;
#endif
        }
    }
}