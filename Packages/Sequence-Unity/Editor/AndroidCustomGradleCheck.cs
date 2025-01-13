using System.Collections.Generic;
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
            _cachedWarnings = new List<string>();
            if (report.summary.platform == BuildTarget.Android)
            {
                SequenceConfig config = SequenceConfig.GetConfig();
                if (config.StoreSessionPrivateKeyInSecureStorage)
                {
                    bool isCustomGradlePropertiesEnabled =
                        EditorPrefs.GetBool("CustomPropertiesGradleTemplateEnabled", false);
                    if (!isCustomGradlePropertiesEnabled)
                    {
                        _cachedWarnings.Add(
                            "Sequence - Custom Gradle Properties Template is not enabled. This may cause issues with secure storage on Android. Refer to: " + _docsUrl);
                    }

                    bool isCustomMainGradleEnabled = EditorPrefs.GetBool("CustomMainGradleTemplateEnabled", false);
                    if (!isCustomMainGradleEnabled)
                    {
                        _cachedWarnings.Add(
                            "Sequence - Custom Main Gradle Template is not enabled. This may cause issues with secure storage on Android. Refer to: " + _docsUrl);
                    }
                    
                    foreach (string warning in _cachedWarnings)
                    {
                        Debug.LogWarning(warning);
                    }

                    if (_cachedWarnings.Count > 0)
                    {
                        WarningPopup.ShowWindow(_cachedWarnings);
                    }
                }
            }
        }

        private class WarningPopup : EditorWindow
        {
            private static List<string> warnings;

            public static void ShowWindow(List<string> warningsToShow)
            {
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
