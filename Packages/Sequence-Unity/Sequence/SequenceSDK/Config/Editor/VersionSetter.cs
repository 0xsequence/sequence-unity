using UnityEditor;
using System.IO;
using UnityEditor.Callbacks;

namespace Sequence.Config.Editor
{
    public static class VersionSetter 
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            InjectSDKVersionIntoResources();
        }

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