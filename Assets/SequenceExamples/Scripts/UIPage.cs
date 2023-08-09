using Sequence.Demo.Tweening;
using UnityEngine;

namespace Sequence.Demo
{
    [RequireComponent(typeof(RectTransform))]
    public class UIPage : MonoBehaviour
    {
        private RectTransform _transform;
        [SerializeField] private float _openAnimationDurationInSeconds;
        [SerializeField] private float _closeAnimationDurationInSeconds;
        private GameObject _gameObject;
        private Scale _scale;

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
            _gameObject = gameObject;
            _scale = _gameObject.AddComponent<Scale>();
            _scale.Initialize(_transform);
            _transform.localScale = new Vector3(0, 0, 0);
        }

        private void Start()
        {
            _gameObject.SetActive(false);
        }

        public void Open()
        {
            _gameObject.SetActive(true);
            _scale.ScaleOverTime(1, _openAnimationDurationInSeconds);
        }

        public void Close()
        {
            _scale.ScaleOverTime(0, _closeAnimationDurationInSeconds);
            Invoke(nameof(Deactivate), _closeAnimationDurationInSeconds);
        }

        private void Deactivate()
        {
            _gameObject.SetActive(false);
        }
    }
}