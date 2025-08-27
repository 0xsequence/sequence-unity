using System.IO;
using UnityEditor;
using UnityEngine;

namespace Sequence.Config
{
    public static class PackageVersionReader
    {
        public static string GetVersion()
        {
            string packagePath = FindPackageJsonPath();

            if (string.IsNullOrEmpty(packagePath) || !File.Exists(packagePath))
            {
                throw new FileNotFoundException("Could not find package.json in any of the expected locations.");
            }

            string json = File.ReadAllText(packagePath);
            return ExtractVersionFromJson(json);
        }


        private static string FindPackageJsonPath()
        {
            if (CheckDirectories(new[] {
                    "Packages/Sequence-Unity/package.json",
                    "Packages/xyz.0xsequence.waas-unity/package.json"
                }, out var availableFile))
            {
                return availableFile;
            }

            string[] directories = Directory.GetDirectories("Library/PackageCache/", "xyz.0xsequence.waas-unity*", SearchOption.TopDirectoryOnly);

            if (directories.Length > 0)
            {
                string packageDir = directories[0];
                string packageJsonPath = Path.Combine(packageDir, "package.json");
                if (File.Exists(packageJsonPath))
                {
                    return packageJsonPath;
                }
            }

            return null;
        }

        private static bool CheckDirectories(string[] files, out string availableFile)
        {
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    availableFile = file;
                    return true;
                }   
            }

            availableFile = null;
            return false;
        }

        private static string ExtractVersionFromJson(string json)
        {
            var jsonObj = JsonUtility.FromJson<PackageJson>(json);
            return jsonObj.version;
        }

        private class PackageJson
        {
            public string version;
        }
    }
}
