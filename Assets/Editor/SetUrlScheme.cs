using System;
using System.IO;
using Sequence.Authentication;
using Sequence.Authentication.ScriptableObjects;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Editor
{
    public class SetUrlScheme
    {
        private static string _plistPath;
        private static string _urlScheme;
        
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            OpenIdAuthenticatorConfig config = Resources.Load<OpenIdAuthenticatorConfig>("OpenIdAuthenticatorConfig");
            if (config == null)
            {
                throw new Exception("OpenIdAuthenticatorConfig not found. Make sure to create and configure it and place it at the root of your Resources folder. Create it from the top bar with Assets > Create > Sequence > OpenIdAuthenticatorConfig");
                return;
            }
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
        }
    }
}