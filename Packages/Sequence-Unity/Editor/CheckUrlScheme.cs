using System;
using System.IO;
using Sequence.Config;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_IOS || UNITY_STANDALONE_OSX
using UnityEditor.iOS.Xcode;
#endif

namespace Sequence.Editor
{
    public class CheckUrlScheme : IPreprocessBuildWithReport
    {
        private static string _plistPath;
        private static string _urlScheme;
        
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            SequenceConfig config = SequenceConfig.GetConfig(SequenceService.None);
            _urlScheme = config.UrlScheme;

            if (string.IsNullOrWhiteSpace(_urlScheme))
            {
                Debug.LogWarning(SequenceConfig.MissingConfigError("Url Scheme").Message);
            }
            
            if (target == BuildTarget.iOS)
            {
                _plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
                CheckPlistUrlScheme();
            }
            else if (target == BuildTarget.StandaloneOSX)
            {
                _plistPath = Path.Combine(pathToBuiltProject, "Contents/Info.plist");
                CheckPlistUrlScheme();
            }
        }

        private static Exception _missingUrlSchemeException = new BuildFailedException(
            "URL Scheme not set in Unity Editor. Please follow the instructions here or social sign in will not work! https://docs.sequence.xyz/sdk/unity/authentication");
        
        private static void CheckPlistUrlScheme()
        {
#if UNITY_STANDALONE_OSX
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(_plistPath);

            PlistElementDict rootDict = plist.root;
            if (!rootDict.values.ContainsKey("CFBundleURLTypes"))
            {
                throw _missingUrlSchemeException;
            }
            if (rootDict.values["CFBundleURLTypes"] is PlistElementArray existingArray)
            {
                if (existingArray.values.Count == 0)
                {
                    throw _missingUrlSchemeException;
                }
                if (existingArray.values[0] is PlistElementDict existingDict)
                {
                    if (!existingDict.values.ContainsKey("CFBundleURLSchemes"))
                    {
                        throw _missingUrlSchemeException;
                    }
                    if (existingDict.values["CFBundleURLSchemes"] is PlistElementArray newArray)
                    {
                        List<PlistElement> values = newArray.values;
                        int count = values.Count;
                        if (count == 0)
                        {
                            throw _missingUrlSchemeException;
                        }
                        for (int i =0; i < count; i++)
                        {
                            PlistElement plistElement = values[i];
                            if (plistElement is PlistElementString plistElementString)
                            {
                                if (plistElementString.value == _urlScheme)
                                {
                                    return;
                                }
                            }
                        }
                        throw _missingUrlSchemeException;
                    }
                }
            }
#endif
        }
        
        public int callbackOrder => 0;
        public void OnPreprocessBuild(BuildReport report)
        {
            SequenceConfig config = SequenceConfig.GetConfig();
            _urlScheme = config.UrlScheme;

            List<string> warnings = new List<string>();

            if (string.IsNullOrWhiteSpace(_urlScheme))
            {
                warnings.Add(SequenceConfig.MissingConfigError("Url Scheme").Message);
            }

            if (_urlScheme.ToLower() != _urlScheme)
            {
                warnings.Add($"{nameof(config.UrlScheme)} should be all lowercase; if uppercase characters are included, you may encounter difficulties with deep-linking on certain platforms.");
            }
            
            if (warnings.Count > 0)
            {
                foreach (var warning in warnings)
                {
                    Debug.LogWarning(warning);
                }
                
                SequenceWarningPopup.ShowWindow(warnings, "https://docs.sequence.xyz/sdk/unity/onboard/authentication/oidc");
            }
        }
    }
}