using System.Collections;
using Sequence.Demo.Tweening;
using UnityEngine;

namespace Sequence.Boilerplates
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TweenAnimation : MonoBehaviour, ITween
    {
        [SerializeField] private float _start = 0.7f;
        [SerializeField] private float _duration = 0.3f;
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
            AnimateIn(_duration);
        }
        
        public void Initialize(RectTransform rectTransform)
        {
            throw new System.NotImplementedException();
        }

        public void AnimateIn(float _)
        {
            gameObject.SetActive(true);
            RestartRoutine(true);
        }

        public void AnimateOut(float _)
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

                transform.localScale = Vector3.LerpUnclamped(Vector3.one * _start, Vector3.one, curveValue);
                yield return null;
            }
            
            if (!forward)
                gameObject.SetActive(false);
        }
    }
}
