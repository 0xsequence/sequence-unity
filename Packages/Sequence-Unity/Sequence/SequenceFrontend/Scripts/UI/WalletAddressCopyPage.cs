using System;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public abstract class WalletAddressCopyPage : UIPage
    {
        [SerializeField] protected TextMeshProUGUI _walletAddressText;
        [SerializeField] protected Image _copyAddressIcon;
        [SerializeField] protected Sprite _checkMarkIcon;
        [SerializeField] protected Sprite _copyIcon;
        [SerializeField] protected float _timeBeforeResettingCopyIcon = 1f;
        
        protected Address _walletAddress;
        
        public override void Open(params object[] args)
        {
            base.Open(args);
            _walletAddress = args.GetObjectOfTypeIfExists<Address>();
            if (_walletAddress == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(Address)} as an argument");
            }
            
            _walletAddressText.text = _walletAddress.CondenseForUI();
            ResetCopyIcon();
        }
        
        /// <summary>
        /// Copy _walletAddress to clipboard and temporarily change _copyAddressIcon's sprite
        /// </summary>
        public void CopyWalletAddress()
        {
            GUIUtility.systemCopyBuffer = _walletAddress.ToString();
            
            _copyAddressIcon.sprite = _checkMarkIcon;
            Invoke(nameof(ResetCopyIcon), _timeBeforeResettingCopyIcon);
        }

        private void ResetCopyIcon()
        {
            _copyAddressIcon.sprite = _copyIcon;
        }
    }
}