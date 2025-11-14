using UnityEditor;
using System.IO;
using UnityEditor.Callbacks;

namespace Sequence.Config.Editor
{
    public static class VersionSetter
    {
        // Because of the InitializeOnLoad attribute, this method will be called whenever code is recompiled in the editor
        
        [DidReloadScripts]
        private static void InjectSDKVersionIntoResources()
        {
            string version = PackageVersionReader.GetVersion();
            string versionFilePath = "Assets/Resources/sequence-unity-version.txt";

            if (!Directory.Exists("Assets/Resources"))
            {
                Directory.CreateDirectory("Assets/Resources");
            }

            File.WriteAllText(versionFilePath, version);
            AssetDatabase.Refresh();
        }
    }
}