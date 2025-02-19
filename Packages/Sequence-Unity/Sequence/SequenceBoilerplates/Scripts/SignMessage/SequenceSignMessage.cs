using System;
using Sequence.EmbeddedWallet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Boilerplates.SignMessage
{
    public class SequenceSignMessage : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Button _signButton;
        [SerializeField] private Button _copyButton;
        [SerializeField] private TMP_Text _signatureText;
        [SerializeField] private TMP_InputField _messageInput;
        [SerializeField] private MessagePopup _messagePopup;
        [SerializeField] private string _initSignatureText;
        
        private IWallet _wallet;
        private Chain _chain;
        private Action _onClose;
        private string _curInput;
        private string _curSignature;

        private void Start()
        {
            _signButton.onClick.AddListener(SignMessage);
            _copyButton.onClick.AddListener(CopySignature);
            _messageInput.onValueChanged.AddListener(VerifyInput);
        }

        /// <summary>
        /// This function is called when the user clicks the close button.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            _onClose?.Invoke();
        }

        /// <summary>
        /// Required function to configure this Boilerplate.
        /// </summary>
        /// <param name="wallet">This Wallet instance will perform transactions.</param>
        /// <param name="chain">Chain used to get balances and send transactions.</param>
        /// <param name="onClose">(Optional) Callback when the user closes this window.</param>
        public void Show(IWallet wallet, Chain chain, Action onClose = null)
        {
            _wallet = wallet;
            _chain = chain;
            _onClose = onClose;
            
            gameObject.SetActive(true);
            _messagePopup.gameObject.SetActive(false);
            _messageInput.text = string.Empty;
            VerifyInput(string.Empty);
        }

        private async void SignMessage()
        {
            _curSignature = await _wallet.SignMessage(_chain, _curInput);
            _signatureText.text = _curSignature;
            _copyButton.interactable = true;
        }

        private void CopySignature()
        {
            GUIUtility.systemCopyBuffer = _curSignature;
            _messagePopup.Show("Copied");
        }
        
        private void VerifyInput(string newValue)
        {
            _curInput = newValue;
            _signButton.interactable = _curInput.Length > 0;
            _signatureText.text = _initSignatureText;
            _copyButton.interactable = false;
        }
    }
}
