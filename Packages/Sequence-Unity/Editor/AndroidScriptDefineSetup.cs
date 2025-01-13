using System.Collections.Generic;
using Sequence.Config;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Sequence.Editor
{
    /// <summary>
    /// This class will set the script define ENABLE_SEQUENCE_ANDROID_SECURE_STORAGE based on the configuration in SequenceConfig.
    /// We wrap our Android secure storage logic in a script define in order to avoid an exception;
    /// if code is present in the build that attempts to access a Java class, Unity Runtime will throw an exception at startup whether that code is reached or not.
    /// </summary>
    public class AndroidScriptDefineSetup : IPreprocessBuildWithReport
    {
        private const string EnableAndroidSecureStorage = "ENABLE_SEQUENCE_ANDROID_SECURE_STORAGE";

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.Android)
            {
                BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(report.summary.platform);
                List<string> defineList;
                string defines = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(targetGroup));
                if (string.IsNullOrWhiteSpace(defines))
                {
                    defineList = new List<string>();
                }
                else
                {
                    defineList = new List<string>(defines.Split(';'));
                }

                SequenceConfig config = SequenceConfig.GetConfig();
                if (config.StoreSessionPrivateKeyInSecureStorage)
                {
                    if (!defineList.Contains(EnableAndroidSecureStorage))
                    {
                        defineList.Add(EnableAndroidSecureStorage);
                    }
                }
                else
                {
                    if (defineList.Contains(EnableAndroidSecureStorage))
                    {
                        defineList.Remove(EnableAndroidSecureStorage);
                    }
                }

                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(targetGroup), string.Join(";", defineList));
            }
        }
    }
}
