using System;
using System.Net.Mime;
using Sequence.Demo.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class ColorSchemeManager : MonoBehaviour
    {
        [SerializeField] private ColorScheme _colorScheme;
        public void ApplyColorScheme()
        {
            ApplyColorSchemeToChildren(transform);
        }

        private void ApplyColorSchemeToChildren(Transform parent)
        {
            int childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = parent.GetChild(i);
                
                SetButtonColor(child);

                SetTextColor(child);

                SetBackgroundColor(child);
                
                ApplyColorSchemeToChildren(child);
            }
        }

        private void SetButtonColor(Transform t)
        {
            Button button = t.GetComponent<Button>();
            if (button != null)
            {
                Image image = button.GetComponent<Image>();
                if (image != null)
                {
                    image.color = _colorScheme.buttonColor;
                }
            }
        }

        private void SetTextColor(Transform t)
        {
            TextMeshProUGUI text = t.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                text.color = _colorScheme.textColor;
            }
        }

        private void SetBackgroundColor(Transform t)
        {
            TMP_InputField inputField = t.GetComponent<TMP_InputField>();
            if (inputField != null)
            {
                Image image = inputField.GetComponent<Image>();
                if (image != null)
                {
                    image.color = _colorScheme.backgroundColor;
                }
            }

            SequenceUI panel = t.GetComponent<SequenceUI>();
            if (panel != null)
            {
                Image image = panel.GetComponent<Image>();
                if (image != null)
                {
                    image.color = _colorScheme.backgroundColor;
                }
            }
        }
    }
}