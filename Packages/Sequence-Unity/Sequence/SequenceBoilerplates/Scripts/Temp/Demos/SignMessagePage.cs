using System;
using Sequence.Utils;
using Sequence.EmbeddedWallet;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class SignMessagePage : DemoPage
    {
        [SerializeField] private TMP_InputField _messageInputField;
        [SerializeField] private TextMeshProUGUI _signatureText;
        
        
        public override void Open(params object[] args)
        {
            base.Open(args);

            _wallet.OnSignMessageComplete += OnMessageSignComplete;
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
        
        private void OnMessageSignComplete(string signature)
        {
            _signatureText.text = signature;
        }
    }
}