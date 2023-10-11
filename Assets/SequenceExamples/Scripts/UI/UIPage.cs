using System.Collections;
using Sequence.Demo.Tweening;
using UnityEngine;

namespace Sequence.Demo
{
    [RequireComponent(typeof(RectTransform))]
    public class UIPage : MonoBehaviour
    {
        private RectTransform _transform;
        [SerializeField] protected float _openAnimationDurationInSeconds;
        [SerializeField] protected float _closeAnimationDurationInSeconds;
        [SerializeField] private AnimationType _animation;
        protected GameObject _gameObject;
        private ITween _animator;

        public enum AnimationType
        {
            ScaleIn,
            FromBottom,
        }

        protected virtual void Awake()
        {
            _transform = GetComponent<RectTransform>();
            _gameObject = gameObject;
            AddAppropriateAnimator();
            _animator.Initialize(_transform);
        }

        protected virtual void Start()
        {
            
        }

        public virtual void Open(params object[] args)
        {
            _gameObject.SetActive(true);
            _animator.Animate( _openAnimationDurationInSeconds);
        }

        public virtual void Close()
        {
            _animator.AnimateOut(_closeAnimationDurationInSeconds);
            Invoke(nameof(Deactivate), _closeAnimationDurationInSeconds);
        }

        private void Deactivate()
        {
            _gameObject.SetActive(false);
        }

        private void AddAppropriateAnimator()
        {
            switch (_animation)
            {
                case AnimationType.ScaleIn:
                    _animator = _gameObject.AddComponent<Scale>();
                    break;
                case AnimationType.FromBottom:
                    _animator = _gameObject.AddComponent<FromBottom>();
                    break;
            }
        }

        public void OverrideAnimationTimes(float newAnimationDurationInSeconds)
        {
            _closeAnimationDurationInSeconds = newAnimationDurationInSeconds;
            _openAnimationDurationInSeconds = newAnimationDurationInSeconds;
        }
    }
}