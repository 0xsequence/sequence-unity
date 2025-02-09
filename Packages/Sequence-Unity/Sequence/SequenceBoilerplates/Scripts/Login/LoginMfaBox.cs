using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceSDK.Demo
{
    public class LoginMfaBox : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Outline _outline;
        [SerializeField] private Color _normalOutline = Color.white;
        [SerializeField] private Color _highlighedOutline = Color.white;

        public void Show(string text, bool highlight)
        {
            _text.text = text;
            _outline.effectColor = highlight ? _highlighedOutline : _normalOutline;
        }
    }
}
