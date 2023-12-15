using System.IO;
using Sequence.Authentication;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Editor
{
    public class SetUrlScheme
    {
        private static string _plistPath;
        
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            _plistPath = Path.Combine(pathToBuiltProject, "Contents/Info.plist");

            if (target == BuildTarget.iOS)
            {
                SetiOSUrlScheme();
            }
            else if (target == BuildTarget.StandaloneOSX)
            {
                SetMacOSUrlScheme();
            }

            Debug.Log($"Custom URL scheme set successfully: {OpenIdAuthenticator.UrlScheme}");
        }
        
        private static void SetiOSUrlScheme() {
            AddPlistValue("CFBundleURLTypes", new[] { new PlistElementDict() });
            AddPlistValue("CFBundleURLTypes[0].CFBundleURLName", new[] { new PlistElementString(OpenIdAuthenticator.UrlScheme) });
            AddPlistValue("CFBundleURLTypes[0].CFBundleURLSchemes", new[] { new PlistElementString(OpenIdAuthenticator.UrlScheme) });
        }

        private static void SetMacOSUrlScheme()
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
                        newArray.values.Add(new PlistElementString(OpenIdAuthenticator.UrlScheme));
                    }
                }
            }

            plist.WriteToFile(_plistPath);
        }

        private static void AddPlistValue(string key, PlistElement[] elements)
        {
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(_plistPath);

            PlistElementDict rootDict = plist.root;
            if (rootDict.values.ContainsKey(key) && rootDict.values[key] is PlistElementArray existingArray)
            {
                existingArray.values.AddRange(elements);
            }
            else
            {
                PlistElementArray newArray = new PlistElementArray();
                newArray.values.AddRange(elements);
                rootDict.values[key] = newArray;
            }

            plist.WriteToFile(_plistPath);
        }
    }
}