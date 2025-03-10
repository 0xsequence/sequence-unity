#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace Sequence.Config.Editor
{
    [InitializeOnLoad]
    public static class VersionSetter
    {
        static VersionSetter()
        {
            UpdateVersionFileOnImport();
        }

        private static void UpdateVersionFileOnImport()
        {
            EditorApplication.delayCall += InjectSDKVersionIntoResources;
        }

        // Because of the InitializeOnLoad attribute, this method will be called whenever code is recompiled in the editor
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

#endif
