using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sequence.Config
{
    [CreateAssetMenu(fileName = "SequenceConfig", menuName = "Sequence/SequenceConfig", order = 1)]
    public class SequenceConfig : ScriptableObject
    {
        [Header("Social Sign In Configuration")]
        public string UrlScheme;
        public string GoogleClientId;
        public string DiscordClientId;
        public string FacebookClientId;
        public string AppleClientId;
        
        [Header("AWS Configuration")]
        public string Region;
        public string IdentityPoolId;
        public string KMSEncryptionKeyId;
        public string CognitoClientId;
        
        [Header("WaaS Configuration")]
        public int WaaSProjectId;
        public string WaaSVersion = "1.0.0";

        [Header("Sequence SDK Configuration")] 
        public string BuilderAPIKey;
        private static SequenceConfig _config;

        public static SequenceConfig GetConfig()
        {
            if (_config == null)
            {
                _config = Resources.Load<SequenceConfig>("SequenceConfig");
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
    }
}