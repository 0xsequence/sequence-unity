using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Sequence.Editor
{
    public static class AppleEncryptionPostBuild
    {
        [PostProcessBuild(999)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.iOS)
                return;

            var plistPath = Path.Combine(pathToBuiltProject, "Info.plist");

            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            var rootDict = plist.root;

            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

            plist.WriteToFile(plistPath);

            Debug.Log("Added ITSAppUsesNonExemptEncryption = false to Info.plist");
        }
    }
}