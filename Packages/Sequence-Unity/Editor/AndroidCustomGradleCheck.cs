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
        
        private const string _docsUrl = "https://docs.sequence.xyz/sdk/unity/onboard/recovering-sessions#android";

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.Android)
            {
                SequenceConfig config = SequenceConfig.GetConfig(SequenceService.None);
                if (!config.StoreSessionPrivateKeyInSecureStorage)
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
                    SequenceWarningPopup.ShowWindow(warnings, _docsUrl);
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
    }
}
