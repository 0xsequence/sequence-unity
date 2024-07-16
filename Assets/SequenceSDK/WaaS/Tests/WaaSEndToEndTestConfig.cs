using System;
using UnityEngine;

namespace Sequence.EmbeddedWallet.Tests
{
    [CreateAssetMenu(fileName = "WaaSEndToEndTestConfig", menuName = "Sequence/WaaSEndToEndTestConfig", order = 2)]
    public class WaaSEndToEndTestConfig : ScriptableObject
    {
        public string PlayFabTitleId;
        public string PlayFabEmail;
        public string PlayFabPassword;
        
        private static WaaSEndToEndTestConfig _config;

        public static WaaSEndToEndTestConfig GetConfig()
        {
            if (_config == null)
            {
                _config = Resources.Load<WaaSEndToEndTestConfig>("WaaSEndToEndTestConfig");
            }

            if (_config == null)
            {
                throw new Exception("WaaSEndToEndTestConfig not found. Make sure to create and configure it and place it at the root of your Resources folder. Create it from the top bar with Assets > Create > Sequence > WaaSEndToEndTestConfig");
            }

            return _config;
        }
    }
}