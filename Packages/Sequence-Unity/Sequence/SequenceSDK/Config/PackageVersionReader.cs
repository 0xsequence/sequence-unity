using System;
using UnityEngine;

namespace Sequence.Config
{
    public static class PackageVersionReader
    {
        private static string _path = "Packages/Sequence-Unity/package.json";
        
        [Serializable]
        private class PackageJson
        {
            public string version;
        }
        
        public static string GetVersion()
        {
            string json = System.IO.File.ReadAllText(_path);
            PackageJson package = JsonUtility.FromJson<PackageJson>(json);
            return package.version;
        }
    }
}