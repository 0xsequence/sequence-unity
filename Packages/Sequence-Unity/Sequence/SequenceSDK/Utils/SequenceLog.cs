using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Sequence.Utils
{
    public static class SequenceLog
    {
        private const string ScriptingDefineSymbol = "SEQUENCE_DEBUG";
        
        [Conditional(ScriptingDefineSymbol)]
        public static void Info(string message)
        {
            Debug.Log(BuildMessage(message));
        }
        
        [Conditional(ScriptingDefineSymbol)]
        public static void Warning(string message)
        {
            Debug.LogWarning(BuildMessage(message));
        }

        [Conditional(ScriptingDefineSymbol)]
        public static void Error(string message)
        {
            Debug.LogError(BuildMessage(message));
        }
        
        [Conditional(ScriptingDefineSymbol)]
        public static void Exception(Exception e)
        {
            Debug.LogError(BuildMessage($"{e.Message} {e.StackTrace}"));
        }

        private static string BuildMessage(string message)
        {
            return $"<color=#f987ff>[Sequence Dev]</color> {message}";
        }
    }
}