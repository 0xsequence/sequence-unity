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
            string packagesPath = "Packages/Sequence-Unity/package.json";
            if (File.Exists(packagesPath))
            {
                return packagesPath;
            }

            string[] directories = Directory.GetDirectories("Library/PackageCache/", "xyz.0xsequence.waas-unity@*", SearchOption.TopDirectoryOnly);

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
