using System;
using System.Collections;
using Sequence.Marketplace;
using Sequence.Pay;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Demo
{
    public class CheckoutPanel : UIPanel
    {
        private QrCodePage _qrCodePage;
        
        protected override void Awake()
        {
            base.Awake();
            _qrCodePage = GetComponentInChildren<QrCodePage>();
        }
        
        public override void Open(params object[] args)
        {
            base.Open(args);
            ICheckoutHelper cart = args.GetObjectOfTypeIfExists<ICheckoutHelper>();
            if (cart == null)
            {
                throw new ArgumentException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(ICheckoutHelper)} as an argument");
            }

            IFiatCheckout fiatCheckout = args.GetObjectOfTypeIfExists<IFiatCheckout>();
            if (fiatCheckout == null)
            {
                Debug.LogWarning(
                    $"{GetType().Name} must be opened with a {typeof(IFiatCheckout)} as an argument in order to use fiat checkout features");
            }
        }

        public void OpenQrCodePage(QrCodeParams qrCodeParams)
        {
            OpenPageOverlaid(_qrCodePage, qrCodeParams, qrCodeParams.DestinationWallet);
        }
    }
}