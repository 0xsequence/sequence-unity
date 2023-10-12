using System;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class TextExtender : MonoBehaviour, ITextSetter
    {
        private TextMeshProUGUI _text;
        private RectTransform _transform;
        [SerializeField] private float _maxWidth;
        [SerializeField] private float _maxHeight;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _transform = GetComponent<RectTransform>();
        }

        public void SetText(string text, bool resizeWidth = false, bool resizeHeight = false)
        {
            _text.text = text;

            if (resizeWidth)
            {
                ResizeTransformWidthToFitText();
            }

            if (resizeHeight)
            {
                ResizeTransformHeightToFitText();
            }
        }

        private void ResizeTransformWidthToFitText()
        {
            float preferredWidth = Mathf.Min(_text.preferredWidth, _maxWidth);
            _transform.sizeDelta = new Vector2(preferredWidth, _transform.sizeDelta.y);
        }

        private void ResizeTransformHeightToFitText()
        {
            float preferredHeight = Mathf.Min(_text.preferredHeight, _maxHeight);
            _transform.sizeDelta = new Vector2(_transform.sizeDelta.x, preferredHeight);
        }
    }
}