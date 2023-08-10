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

        public void Animate(float durationInSeconds)
        {
            StartCoroutine(DoSlideInFromBottom(durationInSeconds));
        }

        private IEnumerator DoSlideInFromBottom(float durationInSeconds)
        {
            float startTime = Time.time;
            float deltaTime = Time.time - startTime;

            while (deltaTime < durationInSeconds)
            {
                float progress = deltaTime / durationInSeconds;
                Vector3 newPosition = Vector3.Lerp(_startPosition, _targetPosition, progress);
                _transform.anchoredPosition = newPosition;
                deltaTime = Time.time - startTime;
                yield return null;
            }

            _transform.anchoredPosition = _targetPosition;
        }
    }
}