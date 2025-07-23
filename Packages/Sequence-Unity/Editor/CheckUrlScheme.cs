using System;
using System.IO;
using Sequence.Config;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using Sequence.Utils;
#if UNITY_IOS || UNITY_STANDALONE_OSX
using UnityEditor.iOS.Xcode;
#endif

namespace Sequence.Editor
{
    public class CheckUrlScheme : IPreprocessBuildWithReport
    {
        private static string _plistPath;
        private static string _pathToBuiltProject;
        
        private static string _urlScheme => UrlSchemeFactory.CreateFromAppIdentifier();
        
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            _pathToBuiltProject = pathToBuiltProject;
            
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
#if UNITY_IOS || UNITY_STANDALONE_OSX
            var plist = new PlistDocument();
            plist.ReadFromFile(_plistPath);

            var rootDict = plist.root;

            const string key = "CFBundleURLTypes";
            var urlTypes = rootDict[key] != null
                ? rootDict[key].AsArray() 
                : rootDict.CreateArray(key);

            var dict = urlTypes.AddDict();
            dict.SetString("CFBundleURLName", Application.identifier);
            var schemes = dict.CreateArray("CFBundleURLSchemes");
            schemes.AddString(_urlScheme);

            plist.WriteToFile(_plistPath);

#if UNITY_IOS
            // Add SafariServices.framework
            var projPath = PBXProject.GetPBXProjectPath(_pathToBuiltProject);
            var proj = new PBXProject();
            proj.ReadFromFile(projPath);

            var targetGuid = proj.GetUnityFrameworkTargetGuid();
            proj.AddFrameworkToProject(targetGuid, "SafariServices.framework", false);
            proj.WriteToFile(projPath);
#endif
            
#endif
        }
        
        public int callbackOrder => 0;
        public void OnPreprocessBuild(BuildReport report)
        {
#if UNITY_ANDROID
            string manifestPath = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");

            if (!File.Exists(manifestPath))
            {
                Debug.LogWarning("AndroidManifest.xml not found in Plugins/Android. Creating a basic one.");
                CreateBasicManifest(manifestPath);
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(manifestPath);

            XmlNamespaceManager nsMgr = new XmlNamespaceManager(doc.NameTable);

            XmlNode appNode = doc.SelectSingleNode("/manifest/application");

            if (appNode == null)
            {
                Debug.LogError("Missing <application> tag in AndroidManifest.xml.");
                return;
            }

            if (HasCustomScheme(appNode, nsMgr))
            {
                Debug.Log("Custom URL scheme already exists in manifest.");
                return;
            }
            
            string androidNs = "http://schemas.android.com/apk/res/android";
            XmlElement intentFilter = doc.CreateElement("intent-filter");
            
            XmlElement action = doc.CreateElement("action");
            action.SetAttribute("name", androidNs, "android.intent.action.VIEW");
            intentFilter.AppendChild(action);

            XmlElement category1 = doc.CreateElement("category");
            category1.SetAttribute("name", androidNs, "android.intent.category.DEFAULT");
            intentFilter.AppendChild(category1);

            XmlElement category2 = doc.CreateElement("category");
            category2.SetAttribute("name", androidNs, "android.intent.category.BROWSABLE");
            intentFilter.AppendChild(category2);
            
            XmlElement data = doc.CreateElement("data");
            data.SetAttribute("scheme", androidNs, _urlScheme);
            intentFilter.AppendChild(data);

            // Attach to main activity
            XmlNodeList activities = appNode.SelectNodes("activity");
            foreach (XmlNode activity in activities)
            {
                XmlAttribute nameAttr = activity.Attributes["android:name"];
                if (nameAttr != null && nameAttr.Value == "com.unity3d.player.UnityPlayerActivity")
                {
                    activity.AppendChild(intentFilter);
                    Debug.Log($"Added custom URL scheme \"{_urlScheme}\" to AndroidManifest.");
                    doc.Save(manifestPath);
                    return;
                }
            }

            Debug.LogWarning("UnityPlayerActivity not found. Could not add URL scheme intent filter.");
#endif
        }
        
        private bool HasCustomScheme(XmlNode appNode, XmlNamespaceManager nsMgr)
        {
            var nodes = appNode.SelectNodes("//intent-filter/data", nsMgr);
            foreach (XmlNode node in nodes)
            {
                var schemeAttr = node.Attributes["android:scheme"];
                if (schemeAttr != null && schemeAttr.Value == _urlScheme)
                    return true;
            }
            return false;
        }
        
        private void CreateBasicManifest(string path)
        {
            var manifestContent = $@"<?xml version=""1.0"" encoding=""utf-8""?>
                <manifest
                        xmlns:android=""http://schemas.android.com/apk/res/android""
                        xmlns:tools=""http://schemas.android.com/tools""
                >
                    <application>
                        <activity
                                android:name=""com.unity3d.player.UnityPlayerActivity""
                                android:theme=""@style/UnityThemeSelector""
                                android:exported=""true"">
                            <intent-filter>
                                <action android:name=""android.intent.action.MAIN"" />
                                <category android:name=""android.intent.category.LAUNCHER"" />
                            </intent-filter>
                            <meta-data android:name=""unityplayer.UnityActivity"" android:value=""true"" />
                        </activity>
                    </application>
                </manifest>";
            
            File.WriteAllText(path, manifestContent);
        }
    }
}