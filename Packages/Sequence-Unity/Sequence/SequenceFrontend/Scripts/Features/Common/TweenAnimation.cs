using System;
using System.Collections;
using UnityEngine;

namespace SequenceSDK.Samples
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TweenAnimation : MonoBehaviour
    {
        [Serializable]
        public enum Type
        {
            LocalScale,
            AnchoredPositionY
        }
        
        [SerializeField] private Type _type;
        [SerializeField] private float _start = 0.6f;
        [SerializeField] private float _duration = 0.25f;
        [SerializeField] private AnimationCurve _animationCurve;
        
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Coroutine _tweenRoutine;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            PlayForward();
        }

        public void PlayForward()
        {
            gameObject.SetActive(true);
            RestartRoutine(true);
        }

        public void PlayBackward()
        {
            RestartRoutine(false);
        }

        private void RestartRoutine(bool forward)
        {
            if (_tweenRoutine != null)
                StopCoroutine(_tweenRoutine);

            _tweenRoutine = StartCoroutine(TweenRoutine(forward));
        }

        private IEnumerator TweenRoutine(bool forward)
        {
            var t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / _duration;
                var v = forward ? t : 1f - t;

                var curveValue = _animationCurve.Evaluate(v);
                _canvasGroup.alpha = curveValue;

                switch (_type)
                {
                    case Type.LocalScale:
                        transform.localScale = Vector3.LerpUnclamped(Vector3.one * _start, Vector3.one, curveValue);
                        break;
                    case Type.AnchoredPositionY:
                        _rectTransform ??= GetComponent<RectTransform>();
                        _rectTransform.anchoredPosition = Vector2.LerpUnclamped(Vector2.up * _start, Vector2.one, curveValue);
                        break;
                }
                
                yield return null;
            }
            
            if (!forward)
                gameObject.SetActive(false);
        }
    }
}
