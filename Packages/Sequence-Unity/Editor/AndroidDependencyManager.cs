using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Linq;
using Sequence.Config;

namespace Sequence.Editor
{
    /// <summary>
    /// When building for Android, some plugins, like our Secure Storage system, require custom gradle files in order to build successfully
    /// If an integrator isn't using the features of one of these plugins, they shouldn't need to use the custom gradle files
    /// This class will exclude any unused plugins from the build (using config from SequenceConfig) so that integrators can build without custom gradle files
    /// </summary>
    public class AndroidDependencyManager : IPreprocessBuildWithReport
    {
        private const string SecureStoragePluginFilename = "AndroidKeyBridge.java";

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
#if UNITY_ANDROID
            BuildTarget target = report.summary.platform;
            SequenceConfig config = SequenceConfig.GetConfig();

            string[] matchingAssets = AssetDatabase.FindAssets(SecureStoragePluginFilename);

            string pluginPath = matchingAssets
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .FirstOrDefault(path => path.EndsWith(SecureStoragePluginFilename));

            if (string.IsNullOrEmpty(pluginPath))
            {
                Debug.LogWarning($"Secure Storage plugin '{SecureStoragePluginFilename}' not found in project.");
                return;
            }

            PluginImporter pluginImporter = AssetImporter.GetAtPath(pluginPath) as PluginImporter;
            if (pluginImporter == null)
            {
                Debug.LogWarning($"Unable to create {nameof(PluginImporter)} instance at path: {pluginPath}");
                return;
            }

            pluginImporter.SetCompatibleWithPlatform(target, config.StoreSessionPrivateKeyInSecureStorage);
            pluginImporter.SaveAndReimport();
            Debug.Log(
                $"Secure Storage plugin compatibility set to {config.StoreSessionPrivateKeyInSecureStorage} for path: {pluginPath}");
#endif
        }
    }
}
