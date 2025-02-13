using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequenceSDK.Samples
{
    public class ImageHighlightAnimation : MonoBehaviour
    {
        [SerializeField] private float _iterationDuration = 1f;
        [SerializeField] private float _iterationDelay = 1f;
        [SerializeField] private Vector2 _minPosition = new Vector2(-100f, 0f);
        [SerializeField] private Vector2 _maxPosition = new Vector2(100f, 0f);

        private RectTransform _rectTransform;
        private Coroutine _waitRoutine;
        private Coroutine _iterationRoutine;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
        }

        private void OnEnable()
        {
            StartIteration();
        }

        private void StartIteration()
        {
            if (_iterationRoutine != null)
                StopCoroutine(_iterationRoutine);

            _iterationRoutine = StartCoroutine(IterationRoutine());
        }

        private IEnumerator IterationRoutine()
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / _iterationDuration;
                _rectTransform.anchoredPosition = Vector2.Lerp(_minPosition, _maxPosition, t);
                
                yield return null;
            }

            yield return new WaitForSeconds(_iterationDelay);
            StartIteration();
        }
    }
}
