using System;
using System.IO;
using Sequence.Config;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Sequence.Editor
{
    public class CreateConfigFileFromEnvironment : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("Running Pre-Build Script: Creating SequenceConfig asset...");

            const string folderPath = "Assets/Resources";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }

            // Only create a new config file if theres an env variable for the access key
            var accessKey = Environment.GetEnvironmentVariable("ACCESS_KEY");
            if (string.IsNullOrEmpty(accessKey))
                return;
            
            var asset = ScriptableObject.CreateInstance<SequenceConfig>();
            asset.BuilderAPIKey = accessKey;
            asset.WaaSConfigKey = Environment.GetEnvironmentVariable("WAAS_KEY");
            asset.UrlScheme = Environment.GetEnvironmentVariable("URL_SCHEME");
            asset.UrlScheme = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            asset.UrlScheme = Environment.GetEnvironmentVariable("APPLE_CLIENT_ID");
            asset.EditorStoreSessionPrivateKeyInSecureStorage = true;
            asset.StoreSessionPrivateKeyInSecureStorage = true;

            const string assetPath = folderPath + "/SequenceConfig.asset";

            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();

            Debug.Log("ScriptableObject created at: " + assetPath);
        }
    }
}
