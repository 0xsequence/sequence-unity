using UnityEngine;

namespace Sequence.Boilerplates
{
    public interface ITween
    {
        public void Initialize(RectTransform rectTransform);
        public void AnimateIn(float durationInSeconds);
        public void AnimateOut(float durationInSeconds);
    }
}