using System;
using System.Numerics;
using Sequence.EmbeddedWallet;
using Sequence.Utils;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public abstract class DemoPage : UIPage
    {
        [SerializeField] protected TextMeshProUGUI _walletAddressText;
        [SerializeField] protected Chain _chain;
        
        protected IWallet _wallet;
        private BigInteger _maxAmount = 0;
        
        public override void Open(params object[] args)
        {
            base.Open(args);
            _wallet = args.GetObjectOfTypeIfExists<IWallet>();
            if (_wallet == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(IWallet)} as an argument");
            }

            _walletAddressText.text = _wallet.GetWalletAddress();
        }
    }
}