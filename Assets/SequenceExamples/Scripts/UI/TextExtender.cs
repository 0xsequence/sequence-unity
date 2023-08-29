using System;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class TextExtender : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        private RectTransform _transform;

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
            float preferredWidth = _text.preferredWidth;
            _transform.sizeDelta = new Vector2(preferredWidth, _transform.sizeDelta.y);
        }

        private void ResizeTransformHeightToFitText()
        {
            float preferredHeight = _text.preferredHeight;
            _transform.sizeDelta = new Vector2(_transform.sizeDelta.x, preferredHeight);
        }
    }
}