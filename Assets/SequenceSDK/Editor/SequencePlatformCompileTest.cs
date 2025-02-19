using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using Sequence.Config;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Sequence.Editor
{
    /// <summary>
    /// This test will build the project for all platforms and check if the build is compiled successfully
    /// </summary>
    public static class SequencePlatformCompileTest
    {
        private const string BuildDirectory = "Builds";
        private static List<string> _failedBuilds;
        
        [MenuItem("Sequence Dev/Platform Compile Test")]
        public static void RunBuildTest()
        {
            BigInteger startEpochTime = new BigInteger(DateTimeOffset.Now.ToUnixTimeSeconds());
            ClearPreviousErrors();
            
            string[] scenes = GetEnabledScenes();

#if UNITY_EDITOR_WIN
            BuildPlatform(BuildTarget.StandaloneWindows64, $"{BuildDirectory}/WindowsBuild", scenes);
#endif
#if UNITY_EDITOR_OSX            
            BuildPlatform(BuildTarget.StandaloneOSX, $"{BuildDirectory}/MacOSBuild", scenes);
            BuildPlatform(BuildTarget.iOS, $"{BuildDirectory}/iOSBuild", scenes);
#endif
            BuildPlatform(BuildTarget.WebGL, $"{BuildDirectory}/WebGLBuild", scenes);
            AndroidBuildTest($"{BuildDirectory}/AndroidBuild", scenes);
            
            Debug.Log("Platform Compile Test Completed. Check the console for errors.");
            foreach (var failedBuild in _failedBuilds)
            {
                Debug.LogError(failedBuild);
            }
            
            BigInteger endEpochTime = new BigInteger(DateTimeOffset.Now.ToUnixTimeSeconds());
            Debug.Log($"Total Test Time: {endEpochTime - startEpochTime} seconds");
        }

        private static void ClearPreviousErrors()
        {
            _failedBuilds = new List<string>();
            
            string errorDirectory = "PlatformCompileTestErrors";
            if (Directory.Exists(errorDirectory))
            {
                var txtFiles = Directory.GetFiles(errorDirectory, "*.txt");
                foreach (var file in txtFiles)
                {
                    File.Delete(file);
                }
            }
            else
            {
                Directory.CreateDirectory(errorDirectory);
            }
        }

        private static string[] GetEnabledScenes()
        {
            return EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();
        }

        private static void BuildPlatform(BuildTarget target, string path, string[] scenes)
        {
            try
            {
                BuildToPlatformAndCheckForSuccess(target, path, scenes);
            }
            finally
            {
                CleanupBuildDirectory();
            }
        }

        private static void BuildToPlatformAndCheckForSuccess(BuildTarget target, string path, string[] scenes)
        {
            BuildReport report = BuildPipeline.BuildPlayer(scenes, path, target, BuildOptions.None);

            if (report.summary.result != BuildResult.Succeeded)
            {
                _failedBuilds.Add($"{target} build failed with {report.summary.totalErrors} errors. Please see {BuildDirectory}/{target}.txt for details.");
                LogErrorsToFile(report, target);
            }
        }

        private static void CleanupBuildDirectory()
        {
            if (Directory.Exists(BuildDirectory))
            {
                Directory.Delete(BuildDirectory, true);
            }
        }

        private static void LogErrorsToFile(BuildReport report, BuildTarget target)
        {
            string errorDirectory = "PlatformCompileTestErrors";
            if (!Directory.Exists(errorDirectory))
            {
                Directory.CreateDirectory(errorDirectory);
            }

            string errorFilePath = Path.Combine(errorDirectory, $"{target}.txt");

            using (StreamWriter writer = new StreamWriter(errorFilePath, false))
            {
                writer.WriteLine($"Build Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine($"Platform: {target}");
                writer.WriteLine($"Build Errors Summary:");
                writer.WriteLine($"Total Errors: {report.summary.totalErrors}");
                writer.WriteLine();
                writer.WriteLine("Detailed Errors:");

                foreach (var step in report.steps)
                {
                    foreach (var message in step.messages)
                    {
                        if (message.type == LogType.Error)
                        {
                            writer.WriteLine($"[{message.type}] {message.content}");
                        }
                    }
                }
            }
        }

        private static void AndroidBuildTest(string path, string[] scenes)
        {
            SequenceConfig config = SequenceConfig.GetConfig(SequenceService.None);
            bool isSecureStorageEnabled = config.StoreSessionPrivateKeyInSecureStorage;
            BuildTarget target = BuildTarget.Android;

            try
            {
                BuildToPlatformAndCheckForSuccess(target, path, scenes);
                
                AssertPluginCompatibility(config, target);
                AssertAppropriateScriptingDefines(config, target);
                
                config.StoreSessionPrivateKeyInSecureStorage = !config.StoreSessionPrivateKeyInSecureStorage;
                
                BuildToPlatformAndCheckForSuccess(target, path, scenes);
                
                AssertPluginCompatibility(config, target);
                AssertAppropriateScriptingDefines(config, target);
            }
            finally
            {
                config.StoreSessionPrivateKeyInSecureStorage = isSecureStorageEnabled;
                
                CleanupBuildDirectory();
            }
        }

        private static void AssertPluginCompatibility(SequenceConfig config, BuildTarget target)
        {
            PluginImporter pluginImporter = AssetImporter.GetAtPath(AndroidDependencyManager.SecureStoragePluginPath) as PluginImporter;
            Assert.IsNotNull(pluginImporter, "Plugin not found at path: " + AndroidDependencyManager.SecureStoragePluginPath);
            Assert.AreEqual(config.StoreSessionPrivateKeyInSecureStorage, pluginImporter.GetCompatibleWithPlatform(target));
        }

        private static void AssertAppropriateScriptingDefines(SequenceConfig config, BuildTarget target)
        {
            string defines = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(target)));
            Assert.AreEqual(config.StoreSessionPrivateKeyInSecureStorage, defines.Contains(AndroidScriptDefineSetup.EnableAndroidSecureStorage));
        }
    }
}