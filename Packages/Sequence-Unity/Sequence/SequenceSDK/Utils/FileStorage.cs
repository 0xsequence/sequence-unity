using System.IO;

namespace Sequence.Utils
{
    public static class FileStorage
    {
        public static void RemoveAllInDirectory(params string[] pathParts)
        {
            var path = Path.Combine(pathParts);
            var dir = new DirectoryInfo(path);

            foreach (var fi in dir.GetFiles())
                fi.Delete();
            
            foreach (var di in dir.GetDirectories())
            {
                RemoveAllInDirectory(di.FullName);
                di.Delete();
            }
        }
        
        public static void Save(byte[] content, string fileName, string path)
        {
            CheckForDirectory(path);
            var filePath = Path.Combine(path, fileName);

            File.WriteAllBytes(filePath, content);
        }
        
        public static byte[] Read(string path)
        {
            var exists = File.Exists(path);
            return exists ? File.ReadAllBytes(path) : null;
        }
        
        private static void CheckForDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
