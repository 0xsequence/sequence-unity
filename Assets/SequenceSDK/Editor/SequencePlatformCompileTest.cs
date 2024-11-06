using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
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
            BuildPlatform(BuildTarget.Android, $"{BuildDirectory}/AndroidBuild", scenes);
            
            Debug.Log("Platform Compile Test Completed. Check the console for errors.");
            foreach (var failedBuild in _failedBuilds)
            {
                Debug.LogError(failedBuild);
            }
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
                BuildReport report = BuildPipeline.BuildPlayer(scenes, path, target, BuildOptions.None);

                if (report.summary.result != BuildResult.Succeeded)
                {
                    _failedBuilds.Add($"{target} build failed with {report.summary.totalErrors} errors. Please see {BuildDirectory}/{target}.txt for details.");
                    LogErrorsToFile(report, target);
                }
            }
            finally
            {
                if (Directory.Exists(BuildDirectory))
                {
                    Directory.Delete(BuildDirectory, true);
                }
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
    }
}