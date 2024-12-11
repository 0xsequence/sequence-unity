using System;
using System.Collections;
using Sequence.Demo.Tweening;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.UIElements;
using Scale = Sequence.Demo.Tweening.Scale;

namespace Sequence.Demo
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIPage : MonoBehaviour
    {
        private RectTransform _transform;
        [SerializeField] protected float _openAnimationDurationInSeconds;
        [SerializeField] protected float _closeAnimationDurationInSeconds;
        [SerializeField] private AnimationType _animation;
        protected GameObject _gameObject;
        protected ITween _animator;
        protected UIPanel _panel;

        public enum AnimationType
        {
            ScaleIn,
            FromBottom,
            ScaleInVertically,
        }

        protected virtual void Awake()
        {
            _transform = GetComponent<RectTransform>();
            _gameObject = gameObject;
            AddAppropriateAnimator();
            _animator.Initialize(_transform);
        }

        public virtual void Open(params object[] args)
        {
            _panel =
                args.GetObjectOfTypeIfExists<UIPanel>();
            if (_panel == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(UIPanel)} as an argument");
            }
            _gameObject.SetActive(true);
            _animator.AnimateIn(_openAnimationDurationInSeconds);
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
                case AnimationType.ScaleInVertically:
                    _animator = _gameObject.AddComponent<ScaleVertically>();
                    break;
                default:
                    throw new NotImplementedException(
                        $"This animation type {_animation} has not been added to {GetType().Name}'s implementation");
            }
        }

        public void OverrideAnimationTimes(float newAnimationDurationInSeconds)
        {
            _closeAnimationDurationInSeconds = newAnimationDurationInSeconds;
            _openAnimationDurationInSeconds = newAnimationDurationInSeconds;
        }

        public virtual void Back()
        {
            _panel.Back();
        }
    }
}