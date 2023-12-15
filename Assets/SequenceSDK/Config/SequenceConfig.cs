using System;
using UnityEngine;

namespace Sequence.Config
{
    [CreateAssetMenu(fileName = "SequenceConfig", menuName = "Sequence/SequenceConfig", order = 1)]
    public class SequenceConfig : ScriptableObject
    {
        [Header("URL Scheme Configuration")]
        public string UrlScheme = "sdk-powered-by-sequence";

        public static SequenceConfig GetConfig()
        {
            SequenceConfig config = Resources.Load<SequenceConfig>("SequenceConfig");

            if (config == null)
            {
                throw new Exception("SequenceConfig not found. Make sure to create and configure it and place it at the root of your Resources folder. Create it from the top bar with Assets > Create > Sequence > SequenceConfig");
            }

            return config;
        }
    }
}