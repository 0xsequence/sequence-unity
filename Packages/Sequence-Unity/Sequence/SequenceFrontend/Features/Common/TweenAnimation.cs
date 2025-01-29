using System.Collections;
using UnityEngine;

namespace SequenceSDK.Samples
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TweenAnimation : MonoBehaviour
    {
        [SerializeField] private float _startScale = 0.6f;
        [SerializeField] private float _duration = 0.25f;
        [SerializeField] private AnimationCurve _animationCurve;

        private CanvasGroup _canvasGroup;
        private Coroutine _tweenRoutine;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            RestartRoutine();
        }

        private void RestartRoutine()
        {
            if (_tweenRoutine != null)
                StopCoroutine(_tweenRoutine);

            _tweenRoutine = StartCoroutine(TweenRoutine());
        }

        private IEnumerator TweenRoutine()
        {
            var t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / _duration;

                var curveValue = _animationCurve.Evaluate(t);
                _canvasGroup.alpha = curveValue;
                transform.localScale = Vector3.LerpUnclamped(Vector3.one * _startScale, Vector3.one, curveValue);
                
                yield return null;
            }
        }
    }
}
