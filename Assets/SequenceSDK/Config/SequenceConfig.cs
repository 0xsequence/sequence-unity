using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sequence.Config
{
    [CreateAssetMenu(fileName = "SequenceConfig", menuName = "Sequence/SequenceConfig", order = 1)]
    public class SequenceConfig : ScriptableObject
    {
        [Header("Social Sign In Configuration")]
        public string UrlScheme = "sdk-powered-by-sequence";
        
        [Header("AWS Configuration")]
        public string Region = "us-east-2";
        public string IdentityPoolId = "us-east-2:42c9f39d-c935-4d5c-a845-5c8815c79ee3";
        public string KMSEncryptionKeyId = "arn:aws:kms:us-east-2:170768627592:key/0fd8f803-9cb5-4de5-86e4-41963fb6043d";
        public string CognitoClientId = "5fl7dg7mvu534o9vfjbc6hj31p";
        
        [Header("WaaS Configuration")]
        public int WaaSProjectId = 9;
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
    }
}