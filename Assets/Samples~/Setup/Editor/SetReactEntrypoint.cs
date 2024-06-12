#if UNITY_WEBGL
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Sequence.Editor
{
    public static class SetReactEntrypoint
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            string buildName = Path.GetFileNameWithoutExtension(pathToBuiltProject);
            
            string dirName = Path.GetDirectoryName(pathToBuiltProject);
            string appTsxPath = Path.Combine(Path.GetDirectoryName(pathToBuiltProject), buildName, "src", "App.tsx");
            
            string appTsxContent = File.ReadAllText(appTsxPath);
            appTsxContent = appTsxContent.Replace("<ReplaceWithDirectoryName>", buildName);
            File.WriteAllText(appTsxPath, appTsxContent);

            Debug.Log("Setup Unity entrypoint for React App successfully");
        }
    }
}
#endif
