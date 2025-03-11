using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sequence.Config;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor;
using UnityEngine;

namespace Sequence.Editor
{
    public class AndroidCustomGradleCheck : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        private static List<string> _cachedWarnings;
        
        private const string _docsUrl = "https://docs.sequence.xyz/sdk/unity/recovering-sessions#android";

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.Android)
            {
                SequenceConfigBase configBase = SequenceConfig.GetConfig(SequenceService.None);
                if (!configBase.StoreSessionPrivateKeyInSecureStorage)
                {
                    return;
                }
                
                List<string> warnings = new List<string>();

                if (!IsCustomGradlePropertiesTemplateEnabled())
                {
                    warnings.Add(
                        "Sequence - Custom Gradle Properties Template is not enabled. This may cause issues with secure storage on Android. Refer to: " + _docsUrl);
                }

                if (!IsCustomMainGradleTemplateEnabled())
                {
                    warnings.Add(
                        "Sequence - Custom Main Gradle Template is not enabled. This may cause issues with secure storage on Android. Refer to: " + _docsUrl);
                }

                foreach (var warning in warnings)
                {
                    Debug.LogWarning(warning);
                }

                if (warnings.Count > 0)
                {
                    WarningPopup.ShowWindow(warnings);
                }
            }
        }

        private bool IsCustomGradlePropertiesTemplateEnabled()
        {
            return GetProjectSettingsFlag("useCustomGradlePropertiesTemplate");
        }

        private bool IsCustomMainGradleTemplateEnabled()
        {
            return GetProjectSettingsFlag("useCustomMainGradleTemplate");
        }

        private bool GetProjectSettingsFlag(string key)
        {
            string projectSettingsPath = Path.Combine(Application.dataPath, "../ProjectSettings/ProjectSettings.asset");

            if (!File.Exists(projectSettingsPath))
            {
                Debug.LogError("ProjectSettings.asset file not found.");
                return false;
            }

            var lines = File.ReadAllLines(projectSettingsPath);
            foreach (var line in lines)
            {
                if (line.Trim().StartsWith(key + ":"))
                {
                    return line.Trim().EndsWith("1");
                }
            }

            return false;
        }

        private class WarningPopup : EditorWindow
        {
            private static List<string> warnings;

            public static void ShowWindow(List<string> warningsToShow)
            {
                if (warningsToShow == null || warningsToShow.Count == 0)
                {
                    return;
                }
                warnings = warningsToShow;
                var window = GetWindow<WarningPopup>("Sequence Build Warnings");
                window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 200);
                window.Show();
            }

            private void OnGUI()
            {
                GUILayout.Label("Warnings Detected", EditorStyles.boldLabel);

                foreach (string warning in warnings)
                {
                    EditorGUILayout.HelpBox(warning, MessageType.Warning);

                    if (GUILayout.Button("Learn More", GUILayout.Width(100)))
                    {
                        Application.OpenURL(_docsUrl);
                    }
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Dismiss", GUILayout.Height(30)))
                {
                    Close();
                }
            }
        }
    }
}
