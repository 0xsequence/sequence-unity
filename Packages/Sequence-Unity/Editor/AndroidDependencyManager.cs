using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Linq;
using Sequence.Config;
using System.IO;

namespace Sequence.Editor
{
    /// <summary>
    /// When building for Android, some plugins, like our Secure Storage system, require custom gradle files in order to build successfully
    /// If an integrator isn't using the features of one of these plugins, they shouldn't need to use the custom gradle files
    /// This class will exclude any unused plugins from the build (using config from SequenceConfig) so that integrators can build without custom gradle files
    /// </summary>
    public class AndroidDependencyManager : IPreprocessBuildWithReport
    {
        public const string SecureStoragePluginFilename = "AndroidKeyBridge.java";

        private const string RelevantDocsUrl =
            "https://docs.sequence.xyz/sdk/unity/onboard/recovering-sessions#android";

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
#if UNITY_ANDROID
            BuildTarget target = report.summary.platform;
            SequenceConfig config = SequenceConfig.GetConfig();

            string[] files = Directory.GetFiles("Assets", SecureStoragePluginFilename, SearchOption.AllDirectories);
            string pluginPath = files.FirstOrDefault();
            if (string.IsNullOrEmpty(pluginPath))
            {
                if (config.StoreSessionPrivateKeyInSecureStorage)
                {
                    ShowWarning($"Secure Storage plugin '{SecureStoragePluginFilename}' not found in project. Please make sure you have imported it via Samples in Package Manager");
                }
                return;
            }

            PluginImporter pluginImporter = AssetImporter.GetAtPath(pluginPath) as PluginImporter;
            if (pluginImporter == null)
            {
                ShowWarning($"Unable to create {nameof(PluginImporter)} instance at path: {pluginPath}");
                return;
            }

            pluginImporter.SetCompatibleWithPlatform(target, config.StoreSessionPrivateKeyInSecureStorage);
            pluginImporter.SaveAndReimport();
            Debug.Log(
                $"Secure Storage plugin compatibility set to {config.StoreSessionPrivateKeyInSecureStorage} for path: {pluginPath}");
#endif
        }

        private void ShowWarning(string warning)
        {
            Debug.LogWarning(warning);
            SequenceWarningPopup.ShowWindow(new List<string>() {warning}, RelevantDocsUrl);
        }
    }
}
