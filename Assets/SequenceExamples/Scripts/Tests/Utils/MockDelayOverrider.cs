using Sequence.Demo;
using UnityEngine;

namespace SequenceExamples.Scripts.Tests.Utils
{
    public class MockDelayOverrider: MonoBehaviour
    {
        public void OverrideAnimationTimes(float newTimeInSeconds)
        {
            UIPage[] pages = FindObjectsOfType<UIPage>();
            int count = pages.Length;
            for (int i = 0; i < count; i++)
            {
                pages[i].OverrideAnimationTimes(newTimeInSeconds);
            }
        }
    }
}