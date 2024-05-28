using System.Collections;
using UnityEngine;

namespace Sequence.Demo.Tweening
{
    public class FromBottom : MonoBehaviour, ITween
    {
        private RectTransform _transform;
        private Vector3 _targetPosition;
        private Vector3 _startPosition;

        public void Initialize(RectTransform rectTransform)
        {
            this._transform = rectTransform;
            _targetPosition = _transform.anchoredPosition;
            _startPosition = new Vector3(_targetPosition.x, -Screen.height);
            _transform.anchoredPosition = _startPosition;
        }

        public void AnimateIn(float durationInSeconds)
        {
            StartCoroutine(DoSlide(durationInSeconds, _startPosition, _targetPosition));
        }

        public void AnimateOut(float durationInSeconds)
        {
            StartCoroutine(DoSlide(durationInSeconds, _targetPosition, _startPosition));
        }

        private IEnumerator DoSlide(float durationInSeconds, Vector3 startPosition, Vector3 targetPosition)
        {
            float startTime = Time.time;
            float deltaTime = Time.time - startTime;

            while (deltaTime < durationInSeconds)
            {
                float progress = deltaTime / durationInSeconds;
                Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, progress);
                _transform.anchoredPosition = newPosition;
                deltaTime = Time.time - startTime;
                yield return null;
            }

            _transform.anchoredPosition = _targetPosition;
        }
    }
}