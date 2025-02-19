using UnityEngine;

namespace Sequence.Boilerplates
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