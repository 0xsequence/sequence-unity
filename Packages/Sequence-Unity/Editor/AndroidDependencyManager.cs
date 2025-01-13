using Sequence.Config;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Sequence.Editor
{
    /// <summary>
    /// When building for Android, some plugins, like our Secure Storage system, require custom gradle files in order to build successfully
    /// If an integrator isn't using the features of one of these plugins, they shouldn't need to use the custom gradle files
    /// This class will exclude any unused plugins from the build (using config from SequenceConfig) so that integrators can build without custom gradle files
    /// </summary>
    public class AndroidDependencyManager : IPreprocessBuildWithReport
    {
        private const string _secureStoragePluginPath = "Packages/xyz.0xsequence.waas-unity/Plugins/Android/AndroidKeyBridge.java";
        
        public int callbackOrder => 0;
        
        public void OnPreprocessBuild(BuildReport report)
        {
#if UNITY_ANDROID
            BuildTarget target = report.summary.platform;
            SequenceConfig config = SequenceConfig.GetConfig();
            
            PluginImporter pluginImporter = AssetImporter.GetAtPath(_secureStoragePluginPath) as PluginImporter;

            if (pluginImporter == null)
            {
                Debug.LogWarning($"Plugin not found at path: {_secureStoragePluginPath}");
                return;
            }
            
            pluginImporter.SetCompatibleWithPlatform(target, config.StoreSessionPrivateKeyInSecureStorage);
            pluginImporter.SaveAndReimport();
#endif
        }
    }
}