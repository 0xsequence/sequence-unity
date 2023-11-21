using System;
using System.Threading.Tasks;
using Sequence.Utils;
using Sequence.WaaS;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class WaaSDemoPage : UIPage
    {
        [SerializeField] private TextMeshProUGUI _resultText;
        
        private WaaSWallet _wallet;
        private Address _address;
        
        public override void Open(params object[] args)
        {
            _wallet =
                args.GetObjectOfTypeIfExists<WaaSWallet>();
            if (_wallet == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(WaaSWallet)} as an argument");
            }
            _gameObject.SetActive(true);
            _animator.AnimateIn( _openAnimationDurationInSeconds);

            SetAddress();
            
            _wallet.OnSignMessageComplete += OnSignMessageComplete;
        }

        private async Task SetAddress()
        {
            var addressReturn = await _wallet.GetWalletAddress(new GetWalletAddressArgs(0));
            _address = new Address(addressReturn.address);
        }
        
        public void SignMessage()
        {
            _wallet.SignMessage(new SignMessageArgs(_address, Chain.Polygon, "Hello World!"));
        }
        
        private void OnSignMessageComplete(SignMessageReturn result)
        {
            _resultText.text = result.signature;
        }
    }
}