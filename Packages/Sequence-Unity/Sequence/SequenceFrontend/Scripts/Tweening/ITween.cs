using UnityEngine;

namespace Sequence.Demo.Tweening
{
    public interface ITween
    {
        public void Initialize(RectTransform rectTransform);
        public void AnimateIn(float durationInSeconds);
        public void AnimateOut(float durationInSeconds);
    }
}