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
    public class SequenceConfig : ScriptableObject, ISequenceConfig
    {
        [field: SerializeField, Header("Social Sign In Configuration - Standalone & Web Platforms")] public string UrlScheme { get; set; }
        [field: SerializeField] public string GoogleClientId { get; set; }
        [field: SerializeField] public string DiscordClientId { get; set; }
        [field: SerializeField] public string FacebookClientId { get; set; }
        [field: SerializeField] public string AppleClientId { get; set; }
        
        [field: SerializeField, Header("Social Sign In Configuration - iOS")] public string GoogleClientIdIOS { get; set; }
        [field: SerializeField] public string DiscordClientIdIOS { get; set; }
        [field: SerializeField] public string FacebookClientIdIOS { get; set; }
        [field: SerializeField] public string AppleClientIdIOS { get; set; }

        [field: SerializeField, Header("WaaS Configuration")] public string WaaSConfigKey { get; set; }
        [field: SerializeField, FormerlySerializedAs("EnableAccountOverride")] public bool EnableMultipleAccountsPerEmail { get; set; } = false;
        [field: SerializeField] public string WaaSVersion { get; set; }

        [field: SerializeField, Header("Sequence SDK Configuration")] public string BuilderAPIKey { get; set; }
        [field: SerializeField] public bool StoreSessionPrivateKeyInSecureStorage { get; set; } = false;
        [field: SerializeField] public bool EditorStoreSessionPrivateKeyInSecureStorage { get; set; } = false;
        
        private static ISequenceConfig _config;
        public static ISequenceConfig GetConfig(SequenceService sequenceService = SequenceService.Unspecified)
        {
            if (_config == null)
            {
                _config = GetAppropriateConfig(sequenceService);
                if (_config == null)
                {
                    throw new Exception("SequenceConfig not found. Make sure to create and configure it and place it at the root of your Resources folder. Create it from the top bar with Assets > Create > Sequence > SequenceConfig");
                }
                
                TextAsset versionFile = Resources.Load<TextAsset>("sequence-unity-version");
                if (versionFile != null)
                {
                    _config.WaaSVersion = $"1 (Unity {versionFile.text})";
                }
                else
                {
                    _config.WaaSVersion = $"1 (Unity {PackageVersionReader.GetVersion()})";
                }
                
#if UNITY_EDITOR && !SEQ_DISABLE_PACKAGE_OVERRIDE
                _config.WaaSVersion = $"1 (Unity {PackageVersionReader.GetVersion()})"; // version file is only updated when building
#endif
            }

            return _config;
        }

        public static void SetConfig(ISequenceConfig config)
        {
            _config = config;
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
            return EditorStoreSessionPrivateKeyInSecureStorage &&
                   TestContext.CurrentTestExecutionContext?.ExecutionStatus != TestExecutionStatus.Running;
#else
            return StoreSessionPrivateKeyInSecureStorage;
#endif
        }
    }
#else
    public class SequenceConfig
    {
        private static ISequenceConfig _config;

        public static ISequenceConfig GetConfig(SequenceService sequenceService = SequenceService.Unspecified)
        {
            return _config;
        }
        
        public static void SetConfig(ISequenceConfig config)
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