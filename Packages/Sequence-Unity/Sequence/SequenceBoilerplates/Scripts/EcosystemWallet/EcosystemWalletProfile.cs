using System;
using Sequence.EcosystemWallet;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sequence.Boilerplates
{
    public class EcosystemWalletProfile : MonoBehaviour
    {
        [SerializeField] private Chain _chain;
        [SerializeField] private TMP_InputField _messageInput;
        [SerializeField] private TMP_Text _walletText;
        [SerializeField] private TMP_Text _signatureText;
        [SerializeField] private Button _signMessageButton;
        [SerializeField] private GameObject _loadingOverlay;
        [SerializeField] private MessagePopup _messagePopup;
        [SerializeField] private GenericObjectPool<SessionWalletTile> _sessionPool;

        private IWallet _wallet;
        private Action _onClose;
        private string _curSignature;
        
        public void Close()
        {
            gameObject.SetActive(false);
            _onClose?.Invoke();
        }
        
        public void Show(IWallet wallet, Action onClose)
        {
            _wallet = wallet;
            _onClose = onClose;
            
            _walletText.text = _wallet.Address;
            _messageInput.text = string.Empty;
            
            gameObject.SetActive(true);
            _messagePopup.gameObject.SetActive(false);
            
            SetLoading(false);
            LoadSessions();
        }
        
        public async void SignMessage()
        {
            var message = _messageInput.text;
            SetLoading(true);

            try
            {
                var signature = await _wallet.SignMessage(_chain, message);
                ShowSignature(signature.signature);
                SetLoading(false);
            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }
        }

        public void CopyWalletAddress()
        {
            CopyText(_wallet.Address.Value);
        }

        public void CopySignature()
        {
            CopyText(_curSignature);
        }
        
        private void CopyText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                _messagePopup.Show("Empty text", true);
                return;
            }
            
            GUIUtility.systemCopyBuffer = text;
            _messagePopup.Show("Copied");
        }

        public void SignOut()
        {
            _wallet.Disconnect();
            gameObject.SetActive(false);
        }
        
        private void ShowSignature(string signature)
        {
            _curSignature = signature;
            _signatureText.text = signature;
            _signMessageButton.interactable = !string.IsNullOrEmpty(_curSignature);
        }
        
        private void ShowError(string error)
        {
            Debug.LogError(error);
            _messagePopup.Show(error, true);
            SetLoading(false);
        }
        
        private void SetLoading(bool value)
        {
            _loadingOverlay.SetActive(value);
        }
        
        private void LoadSessions()
        {
            _sessionPool.Cleanup();
            foreach (var wallet in _wallet.GetAllSigners())
                _sessionPool.GetObject().Apply(wallet);
        }
    }
}
