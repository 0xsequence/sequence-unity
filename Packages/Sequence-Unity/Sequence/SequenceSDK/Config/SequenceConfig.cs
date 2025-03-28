using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEditor;
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

        [Header("WaaS Configuration")]
        public string WaaSConfigKey;
        [FormerlySerializedAs("EnableAccountOverride")] public bool EnableMultipleAccountsPerEmail = false;
        public string WaaSVersion { get; private set; }

        [Header("Sequence SDK Configuration")] 
        public string BuilderAPIKey;
        public bool StoreSessionPrivateKeyInSecureStorage = false;
        public bool EditorStoreSessionPrivateKeyInSecureStorage = false;
        
        private static Dictionary<SequenceService, SequenceConfig> _configs = new Dictionary<SequenceService, SequenceConfig>();
        public static SequenceConfig GetConfig(SequenceService sequenceService = SequenceService.Unspecified)
        {
            if (_configs.TryGetValue(sequenceService, out SequenceConfig cachedConfig))
            {
                return cachedConfig;
            }
            else 
            {
                _configs[sequenceService] = GetAppropriateConfig(sequenceService);
                SequenceConfig config = _configs[sequenceService];
                if (config == null)
                {
                    throw new Exception("SequenceConfig not found. Make sure to create and configure it and place it at the root of your Resources folder. Create it from the top bar with Assets > Create > Sequence > SequenceConfig");
                }
                
                TextAsset versionFile = Resources.Load<TextAsset>("sequence-unity-version");
                if (versionFile != null)
                {
                    config.WaaSVersion = $"1 (Unity {versionFile.text})";
                }
                else
                {
                    config.WaaSVersion = $"1 (Unity {PackageVersionReader.GetVersion()})";
                }
                
#if UNITY_EDITOR && !SEQ_DISABLE_PACKAGE_OVERRIDE
                config.WaaSVersion = $"1 (Unity {PackageVersionReader.GetVersion()})"; // version file is only updated when building
#endif
                return config;
            }
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

        public static ConfigJwt GetConfigJwt(SequenceService service = SequenceService.Unspecified)
        {
            SequenceConfig config = GetConfig(service);
            return GetConfigJwt(config);
        }

        public static ConfigJwt GetConfigJwt(SequenceConfig config)
        {
            string configKey = config.WaaSConfigKey;
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
}