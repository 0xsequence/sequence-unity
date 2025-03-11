using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_2017_1_OR_NEWER
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Serialization;
#endif

namespace Sequence.Config
{
#if UNITY_2017_1_OR_NEWER
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

        [Header("WaaS Configuration")]
        public string WaaSConfigKey;
        [FormerlySerializedAs("EnableAccountOverride")] public bool EnableMultipleAccountsPerEmail = false;
        public string WaaSVersion;

        [Header("Sequence SDK Configuration")]
        public string BuilderAPIKey;
        public bool StoreSessionPrivateKeyInSecureStorage = false;
        public bool EditorStoreSessionPrivateKeyInSecureStorage = false;
        
        private static SequenceConfigBase _configBase;
        public static SequenceConfigBase GetConfig(SequenceService sequenceService = SequenceService.Unspecified)
        {
            if (_configBase == null)
            {
                var config = GetAppropriateConfig(sequenceService);
                _configBase = new SequenceConfigBase
                {
                    UrlScheme = config.UrlScheme,
                    GoogleClientId = config.GoogleClientId,
                    DiscordClientId = config.DiscordClientId,
                    FacebookClientId = config.FacebookClientId,
                    AppleClientId = config.AppleClientId,
                    GoogleClientIdIOS = config.GoogleClientIdIOS,
                    DiscordClientIdIOS = config.DiscordClientIdIOS,
                    FacebookClientIdIOS = config.FacebookClientIdIOS,
                    AppleClientIdIOS = config.AppleClientIdIOS,
                    WaaSConfigKey = config.WaaSConfigKey,
                    WaaSVersion = config.WaaSVersion,
                    BuilderAPIKey =  config.BuilderAPIKey,
                    EnableMultipleAccountsPerEmail = config.EnableMultipleAccountsPerEmail,
                    StoreSessionPrivateKeyInSecureStorage = config.StoreSessionPrivateKeyInSecureStorage,
                    EditorStoreSessionPrivateKeyInSecureStorage = config.EditorStoreSessionPrivateKeyInSecureStorage
                };
                
                if (_configBase == null)
                {
                    throw new Exception("SequenceConfig not found. Make sure to create and configure it and place it at the root of your Resources folder. Create it from the top bar with Assets > Create > Sequence > SequenceConfig");
                }
                
                TextAsset versionFile = Resources.Load<TextAsset>("sequence-unity-version");
                if (versionFile != null)
                {
                    _configBase.WaaSVersion = $"1 (Unity {versionFile.text})";
                }
                else
                {
                    _configBase.WaaSVersion = $"1 (Unity {PackageVersionReader.GetVersion()})";
                }
                
#if UNITY_EDITOR && !SEQ_DISABLE_PACKAGE_OVERRIDE
                _configBase.WaaSVersion = $"1 (Unity {PackageVersionReader.GetVersion()})"; // version file is only updated when building
#endif
            }

            return _configBase;
        }

        public static void SetConfig(SequenceConfigBase configBase)
        {
            _configBase = configBase;
        }

        private static SequenceConfig GetAppropriateConfig(SequenceService sequenceService)
        {
            switch (sequenceService)
            {
                case SequenceService.WaaS:
#if SEQUENCE_DEV_WAAS || SEQUENCE_DEV
                    return LoadDevConfig();
#else
                    return LoadProdConfig();
#endif
                case SequenceService.Indexer:
#if SEQUENCE_DEV_INDEXER || SEQUENCE_DEV
                    return LoadDevConfig();
#else
                    return LoadProdConfig();
#endif
                case SequenceService.NodeGateway:
#if SEQUENCE_DEV_NODEGATEWAY || SEQUENCE_DEV
                    return LoadDevConfig();
#else
                    return LoadProdConfig();
#endif
                case SequenceService.Marketplace:
#if SEQUENCE_DEV_MARKETPLACE || SEQUENCE_DEV
                    return LoadDevConfig();
#else
                    return LoadProdConfig();
#endif
                case SequenceService.Stack:
#if SEQUENCE_DEV_STACK || SEQUENCE_DEV
                    return LoadDevConfig();
#else
                    return LoadProdConfig();
#endif
                
                default:
#if SEQUENCE_DEV
                    return LoadDevConfig();
#else
                    return LoadProdConfig();
#endif
            }
        }

        private static SequenceConfig LoadDevConfig()
        {
            return LoadConfig("SequenceDevConfig");
        }

        private static SequenceConfig LoadProdConfig()
        {
            return LoadConfig("SequenceConfig");
        }

        private static SequenceConfig LoadConfig(string configName)
        {
#if UNITY_EDITOR
            string[] guids = AssetDatabase.FindAssets($"{configName} t:{nameof(SequenceConfig)}");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.StartsWith("Assets"))
                {
                    return AssetDatabase.LoadAssetAtPath<SequenceConfig>(path);
                }
            }

            return Resources.Load<SequenceConfig>(configName);
#else
            return Resources.Load<SequenceConfig>(configName);
#endif
        }

        public static Exception MissingConfigError(string valueName)
        {
            return new Exception($"{valueName} is not set. Please set it in SequenceConfig asset in your Resources folder.");
        }

        public static ConfigJwt GetConfigJwt()
        {
            string configKey = _configBase.WaaSConfigKey;
            if (string.IsNullOrWhiteSpace(configKey))
            {
                throw SequenceConfig.MissingConfigError("WaaS Config Key");
            }

            return JwtHelper.GetConfigJwt(configKey);
        }
    }
#else
    public class SequenceConfig
    {
        private static SequenceConfigBase _config;

        public static SequenceConfigBase GetConfig(SequenceService sequenceService = SequenceService.Unspecified)
        {
            return _config;
        }
        
        public static void SetConfig(SequenceConfigBase config)
        {
            _config = config;
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
    }
#endif
}