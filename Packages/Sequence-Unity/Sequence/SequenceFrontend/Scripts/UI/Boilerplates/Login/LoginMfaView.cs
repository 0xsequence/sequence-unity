using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Boilerplates.Login
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
            OnSelect(string.Empty);
            StartCoroutine(WaitSelect());
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

        // We need to wait one frame here otherwise the input field is not selected automatically
        private IEnumerator WaitSelect()
        {
            yield return 0;
            _codeInput.Select();
            _codeInput.ActivateInputField();
        }
    }
}
