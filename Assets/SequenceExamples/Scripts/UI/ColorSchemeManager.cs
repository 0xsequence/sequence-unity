#if UNITY_EDITOR
using Sequence.Demo.ScriptableObjects;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class ColorSchemeManager : MonoBehaviour
    {
        [SerializeField] private ColorScheme _colorScheme;
        [SerializeField] private GameObject[] _uiElementPrefabs;
        public void ApplyColorScheme()
        {
            ApplyColorSchemeToChildren(transform);
            int prefabs = _uiElementPrefabs.Length;
            for (int i = 0; i < prefabs; i++)
            {
                GameObject prefabInstance = PrefabUtility.InstantiatePrefab(_uiElementPrefabs[i]) as GameObject;
                ApplyColorSchemeToChildren(prefabInstance.transform);
                PrefabUtility.ApplyPrefabInstance(prefabInstance, InteractionMode.UserAction);
                DestroyImmediate(prefabInstance);
            }
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
            if (text != null && text.color.a > 0.01f) // Transparent text should remain transparent
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

            UIPanel panel = t.GetComponent<UIPanel>();
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
#endif