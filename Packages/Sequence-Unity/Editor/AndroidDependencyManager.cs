using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Linq;
using Sequence.Config;
using System.IO;
using System.Text.RegularExpressions;

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
        public const string SequenceGoogleSignInPluginFilename = "GoogleSignInPlugin.java";
        public const string GradleTemplateFilename = "mainTemplate.gradle";

        private static string[] GradleDependencies = new[]
        {
            "androidx.security:security-crypto",
            "com.google.android.gms:play-services-auth",
            "androidx.credentials:credentials",
            "androidx.credentials:credentials-play-services-auth",
            "com.google.android.libraries.identity.googleid:googleid",
        };
        
        private const string RelevantDocsUrl =
            "https://docs.sequence.xyz/sdk/unity/onboard/recovering-sessions#android";

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
#if UNITY_ANDROID
            var target = report.summary.platform;
            var config = SequenceConfig.GetConfig();
            
            CheckPlugin(SecureStoragePluginFilename, config.StoreSessionPrivateKeyInSecureStorage, target);
            CheckPlugin(SequenceGoogleSignInPluginFilename, true, target);
            CheckGradleTemplateDependencies();
            AssetDatabase.Refresh();
#endif
        }

        [MenuItem("Sequence/Android Plugins")]
        public static void Test()
        {
            CheckPlugin(SecureStoragePluginFilename, true, BuildTarget.Android);
            CheckPlugin(SequenceGoogleSignInPluginFilename, true, BuildTarget.Android);
        }

        private static void CheckPlugin(string fileName, bool enable, BuildTarget platform)
        {
            var existingFiles = Directory.GetFiles("Assets", fileName, SearchOption.AllDirectories);
            var pluginPath = existingFiles.FirstOrDefault();
            
            if (string.IsNullOrEmpty(pluginPath))
            {
                if (enable)
                    TryCopyPlugin(fileName);
                
                return;
            }

            var pluginImporter = AssetImporter.GetAtPath(pluginPath) as PluginImporter;
            if (!pluginImporter)
            {
                ShowWarning($"Unable to create {nameof(PluginImporter)} instance at path: {pluginPath}");
                return;
            }

            pluginImporter.SetCompatibleWithPlatform(platform, enable);
            pluginImporter.SaveAndReimport();
            
            Debug.Log($"Plugin {fileName} compatibility set to {enable} for path: {pluginPath}");
        }

        private static void TryCopyPlugin(string fileName)
        {
            var targetPath = Path.Combine(Application.dataPath, "Plugins/Android", fileName);
            var sourcePath = FindFileInPackages("Plugins/Android/" + fileName);
            
            var directory = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.Copy(sourcePath, targetPath);
        }
        
        private static void CheckGradleTemplateDependencies()
        {
            var existingFiles = Directory.GetFiles("Assets", GradleTemplateFilename, SearchOption.AllDirectories);
            var gradleFilePath = existingFiles.FirstOrDefault();
            
            if (string.IsNullOrEmpty(gradleFilePath) || !File.Exists(gradleFilePath))
            {
                ShowWarning("mainTemplate.gradle does not exist. Trigger a build once to generate it.");
                return;
            }

            var content = File.ReadAllText(gradleFilePath);
            foreach (var dep in GradleDependencies)
            {
                if (!content.Contains(dep))
                {
                    ShowWarning($"{GradleTemplateFilename} does not include '{dep}' as a dependency.");
                }
            }
        }

        private static void ShowWarning(string warning)
        {
            Debug.LogWarning(warning);
            SequenceWarningPopup.ShowWindow(new List<string>() {warning}, RelevantDocsUrl);
        }

        private static string FindFileInPackages(string relativeFilePath)
        {
            var filePath = $"Packages/Sequence-Unity/{relativeFilePath}";
            if (File.Exists(filePath))
                return filePath;

            var directories = Directory.GetDirectories("Library/PackageCache/", "xyz.0xsequence.waas-unity*", SearchOption.TopDirectoryOnly);
            if (directories.Length > 0)
            {
                var packageDir = directories[0];
                var packageJsonPath = Path.Combine(packageDir, relativeFilePath);
                if (File.Exists(packageJsonPath))
                {
                    return packageJsonPath;
                }
            }
            
            ShowWarning("Plugin file not found: " + relativeFilePath);
            return null;
        }
    }
}
