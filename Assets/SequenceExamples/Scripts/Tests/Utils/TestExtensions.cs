using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace SequenceExamples.Scripts.Tests.Utils
{
    public static class TestExtensions
    {
        public static void AssertImageWithNameHasSprite(Transform parent, string name, Sprite sprite)
        {
            Transform imageTransform = parent.FindAmongDecendants(name);
            Assert.IsNotNull(imageTransform);
            Image image = imageTransform.GetComponent<Image>();
            Assert.IsNotNull(image);
            Assert.AreEqual(sprite, image.sprite);
        }

        public static void AssertTextWithNameHasText(Transform parent, string name, string expected)
        {
            Transform textTransform = parent.FindAmongDecendants(name);
            Assert.IsNotNull(textTransform);
            TextMeshProUGUI text = textTransform.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(text);
            Assert.AreEqual(expected, text.text);
        }
        
        public static void AssertStartsWith(this string value, string startsWith)
        {
            int length = startsWith.Length;
            Assert.AreEqual(startsWith, value.Substring(0, length));
        }

        public static void ClickButtonWithName(Transform parent, string name)
        {
            Transform buttonTransform = parent.FindAmongDecendants(name);
            Assert.IsNotNull(buttonTransform);
            Button button = buttonTransform.GetComponent<Button>();
            Assert.IsNotNull(button);
            button.onClick.Invoke();
        }
    }
}