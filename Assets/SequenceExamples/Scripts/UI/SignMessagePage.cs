using System;
using Sequence.Utils;
using Sequence.WaaS;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class SignMessagePage : UIPage
    {
        [SerializeField] private TMP_InputField _messageInputField;
        [SerializeField] private TextMeshProUGUI _signatureText;
        [SerializeField] private TextMeshProUGUI _walletAddressText;
        [SerializeField] private Chain _chain;
        
        private IWallet _wallet;
        
        public override void Open(params object[] args)
        {
            base.Open(args);
            _wallet = args.GetObjectOfTypeIfExists<IWallet>();
            if (_wallet == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(IWallet)} as an argument");
            }

            _wallet.OnSignMessageComplete += OnMessageSignComplete;

            _walletAddressText.text = _wallet.GetWalletAddress();
        }
        
        public override void Close()
        {
            base.Close();
            
            _wallet.OnSignMessageComplete -= OnMessageSignComplete;
        }
        
        public void SignMessage()
        {
            _wallet.SignMessage(_chain, _messageInputField.text);
        }

        private void OnMessageSignComplete(SignMessageReturn signMessageReturn)
        {
            _signatureText.text = signMessageReturn.signature;
        }
    }
}