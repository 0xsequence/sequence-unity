using System;
using System.IO;
using Sequence.Config;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
#if UNITY_IOS || UNITY_STANDALONE_OSX
using UnityEditor.iOS.Xcode;
#endif

namespace Editor
{
    public class SetUrlScheme
    {
        private static string _plistPath;
        private static string _urlScheme;
        
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            SequenceConfig config = SequenceConfig.GetConfig();
            _urlScheme = config.UrlScheme;
            
            if (target == BuildTarget.iOS)
            {
                _plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
                SetPlistUrlScheme();
            }
            else if (target == BuildTarget.StandaloneOSX)
            {
                _plistPath = Path.Combine(pathToBuiltProject, "Contents/Info.plist");
                SetPlistUrlScheme();
            }

            Debug.Log($"Custom URL scheme set successfully: {_urlScheme}");
        }

        private static void SetPlistUrlScheme()
        {
#if UNITY_IOS || UNITY_STANDALONE_OSX
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(_plistPath);

            PlistElementDict rootDict = plist.root;
            if (!rootDict.values.ContainsKey("CFBundleURLTypes")) 
            {
                rootDict.values.Add("CFBundleURLTypes", new PlistElementArray());
            }
            if (rootDict.values["CFBundleURLTypes"] is PlistElementArray existingArray)
            {
                if (existingArray.values.Count == 0)
                {
                    existingArray.values.Add(new PlistElementDict());
                }
                if (existingArray.values[0] is PlistElementDict existingDict)
                {
                    if (!existingDict.values.ContainsKey("CFBundleURLSchemes"))
                    {
                        existingDict.values.Add("CFBundleURLSchemes", new PlistElementArray());
                    }
                    if (existingDict.values["CFBundleURLSchemes"] is PlistElementArray newArray)
                    {
                        newArray.values.Add(new PlistElementString(_urlScheme));
                    }
                }
            }

            plist.WriteToFile(_plistPath);
#endif
        }
    }
}