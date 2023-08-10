
using System.Collections;
using UnityEngine;

namespace Sequence.Demo.Tweening
{
    public class Scale : MonoBehaviour, ITween
    {
        private RectTransform _transform;

        public void Initialize(RectTransform rectTransform)
        {
            this._transform = rectTransform;
            _transform.localScale = new Vector3(0, 0, 0);
        }

        public void Animate(float durationInSeconds)
        {
            ScaleOverTime(1, durationInSeconds);
        }

        private void ScaleOverTime(float target, float durationInSeconds)
        {
            Vector3 startingScale = _transform.localScale;
            Vector3 targetScale = new Vector3(target, target, 1);

            StartCoroutine(DoScale(startingScale, targetScale, durationInSeconds));
        }

        private IEnumerator DoScale(Vector3 startingScale, Vector3 targetScale, float durationInSeconds)
        {
            float startTime = Time.time;
            float deltaTime = Time.time - startTime;

            while (deltaTime < durationInSeconds)
            {
                float progress = deltaTime / durationInSeconds;
                Vector3 newScale = Vector3.Lerp(startingScale, targetScale, progress);
                _transform.localScale = newScale;
                deltaTime = Time.time - startTime;
                yield return null;
            }

            _transform.localScale = targetScale;
        }
    }
}