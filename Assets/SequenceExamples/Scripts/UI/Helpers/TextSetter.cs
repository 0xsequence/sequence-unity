using TMPro;

namespace Sequence.Demo
{
    public class TextSetter : ITextSetter
    {
        private TextMeshProUGUI _text;

        public TextSetter(TextMeshProUGUI text)
        {
            _text = text;
        }

        public static implicit operator TextSetter(TextMeshProUGUI text)
        {
            return new TextSetter(text);
        }

        public void SetText(string text, bool resizeWidth = false, bool resizeHeight = false)
        {
            _text.text = text;
        }
    }
}