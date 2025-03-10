namespace Sequence.EmbeddedWallet
{
    public static class LogHandler
    {
        public static void Info(string message)
        {
#if UNITY_2017_1_OR_NEWER
            UnityEngine.Debug.Log(message);
#else
            System.Console.WriteLine(message);
#endif
        }
        
        public static void Warning(string message)
        {
#if UNITY_2017_1_OR_NEWER
            UnityEngine.Debug.LogWarning(message);
#else
            System.Console.WriteLine(message);
#endif
        }
        
        public static void Error(string message)
        {
#if UNITY_2017_1_OR_NEWER
            UnityEngine.Debug.LogError(message);
#else
            System.Console.WriteLine(message);
#endif
        }
    }
}