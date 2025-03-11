
namespace Sequence.Utils
{
    public static class AppEnvironment
    {
        // CompanyName, ProductName, DataPath are used in the secure storage logic, which is disabled in .NET projects anyways
            
#if UNITY_2017_1_OR_NEWER
        public static string CompanyName => UnityEngine.Application.companyName;
        public static string ProductName => UnityEngine.Application.productName;
        public static string DataPath => UnityEngine.Application.persistentDataPath;
        public static float Time => UnityEngine.Time.time;
#else
        public static string CompanyName => "undefined";
        public static string ProductName => "undefined";
        public static string DataPath => "undefined";
        public static float Time => System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
#endif
        
        public static void OpenUrl(string url)
        {
#if UNITY_2017_1_OR_NEWER
            UnityEngine.Application.OpenURL(url);
#endif
        }

        public static int GetRandomNumber(int min, int max)
        {
#if UNITY_2017_1_OR_NEWER
            return UnityEngine.Random.Range(min, max);
#else
            return new System.Random().Next(min, max);
#endif
        }
    }
}