using UnityEngine;

namespace Sequence.Demo.Utils
{
    public static class MonoBehaviourExtensions
    {
        public static void ForceStopCoroutine(this MonoBehaviour monoBehaviour, ref Coroutine coroutine)
        {
            if (coroutine == null)
                return;
            
            monoBehaviour.StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}