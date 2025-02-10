using SequenceSDK.Demo;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceSDK.Samples
{
    public class LoginMfaView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _codeInput;
        [SerializeField] private TMP_Text _errorText;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private LoginMfaBox[] _boxes;

        private bool _selected;

        private void Awake()
        {
            _codeInput.onValueChanged.AddListener(OnInputValueChanged);
            _codeInput.onSelect.AddListener(OnSelect);
            _codeInput.onDeselect.AddListener(OnDeselect);
        }
        
        private void OnEnable()
        {
            _codeInput.text = string.Empty;
            _codeInput.Select();
            OnInputValueChanged(string.Empty);
        }
        
        private void Update()
        {
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand))
                && Input.GetKeyDown(KeyCode.V))
            {
                _codeInput.text = GUIUtility.systemCopyBuffer;
            }
        }

        private void OnInputValueChanged(string newValue)
        {
            if (newValue.Length > _boxes.Length)
            {
                newValue = newValue.Substring(0, _boxes.Length);
                _codeInput.text = newValue;
            }

            _confirmButton.interactable = newValue.Length == _boxes.Length;
            _errorText.text = string.Empty;

            for (var i = 0; i < + _boxes.Length; i++)
            {
                var text = i < newValue.Length ? newValue[i].ToString() : string.Empty;
                _boxes[i].Show(text, _selected && (i == newValue.Length ||
                                                   (newValue.Length == _boxes.Length && newValue.Length - 1 == i)));
            }
        }

        private void OnSelect(string value)
        {
            _selected = true;
            OnInputValueChanged(value);
        }

        private void OnDeselect(string value)
        {
            _selected = false;
            OnInputValueChanged(value);
        }
    }
}
