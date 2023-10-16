using System;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class WalletDropdown : UIPage
    {
        [SerializeField] private TextMeshProUGUI _walletAddressText;
        [SerializeField] private Image _copyAddressIcon;
        [SerializeField] private Sprite _checkMarkIcon;
        [SerializeField] private Sprite _copyIcon;
        [SerializeField] private float _timeBeforeResettingCopyIcon = 1f;
        
        private Address _walletAddress;
        
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
        /// Copy _walletAddress to clipboard
        /// </summary>
        public void CopyAddress()
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