
namespace Sequence.Utils
{
    public static class AppEnvironment
    {
#if UNITY_2017_1_OR_NEWER
        public static string CompanyName => UnityEngine.Application.companyName;
        public static string ProductName => UnityEngine.Application.productName;
        public static string DataPath => UnityEngine.Application.persistentDataPath;
        public static float Time => UnityEngine.Time.time;
#else
        public static string CompanyName => "";
        public static string ProductName => "";
        public static string DataPath => "";
        public static float Time = 0;
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