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
            string projectDir = Path.GetDirectoryName(pathToBuiltProject);
            string buildDir = Path.Combine(projectDir, buildName, "Build");
            string appTsxPath = Path.Combine(projectDir, buildName, "src", "App.tsx");

            string loaderUrl = $"{buildName}.loader.js";
            string dataUrl = $"{buildName}.data";
            string frameworkUrl = $"{buildName}.framework.js";
            string codeUrl = $"{buildName}.wasm";

            // Check if the .gz or .br files exist and update the URLs accordingly
            if (File.Exists(Path.Combine(buildDir, loaderUrl + ".br")))
            {
                loaderUrl += ".br";
            }
            else if (File.Exists(Path.Combine(buildDir, loaderUrl + ".gz")))
            {
                loaderUrl += ".gz";
            }
            else if (File.Exists(Path.Combine(buildDir, loaderUrl + ".unityweb")))
            {
                loaderUrl += ".unityweb";
            }

            if (File.Exists(Path.Combine(buildDir, dataUrl + ".br")))
            {
                dataUrl += ".br";
            }
            else if (File.Exists(Path.Combine(buildDir, dataUrl + ".gz")))
            {
                dataUrl += ".gz";
            }
            else if (File.Exists(Path.Combine(buildDir, dataUrl + ".unityweb")))
            {
                dataUrl += ".unityweb";
            }

            if (File.Exists(Path.Combine(buildDir, frameworkUrl + ".br")))
            {
                frameworkUrl += ".br";
            }
            else if (File.Exists(Path.Combine(buildDir, frameworkUrl + ".gz")))
            {
                frameworkUrl += ".gz";
            }
            else if (File.Exists(Path.Combine(buildDir, frameworkUrl + ".unityweb")))
            {
                frameworkUrl += ".unityweb";
            }

            if (File.Exists(Path.Combine(buildDir, codeUrl + ".br")))
            {
                codeUrl += ".br";
            }
            else if (File.Exists(Path.Combine(buildDir, codeUrl + ".gz")))
            {
                codeUrl += ".gz";
            }
            else if (File.Exists(Path.Combine(buildDir, codeUrl + ".unityweb")))
            {
                codeUrl += ".unityweb";
            }

            string appTsxContent = File.ReadAllText(appTsxPath);
            appTsxContent = appTsxContent.Replace("Build/<ReplaceWithDirectoryName>.loader.js", $"Build/{loaderUrl}");
            appTsxContent = appTsxContent.Replace("Build/<ReplaceWithDirectoryName>.data", $"Build/{dataUrl}");
            appTsxContent = appTsxContent.Replace("Build/<ReplaceWithDirectoryName>.framework.js", $"Build/{frameworkUrl}");
            appTsxContent = appTsxContent.Replace("Build/<ReplaceWithDirectoryName>.wasm", $"Build/{codeUrl}");
            File.WriteAllText(appTsxPath, appTsxContent);

            Debug.Log("Setup Unity entrypoint for React App successfully");
        }
    }
}
#endif
