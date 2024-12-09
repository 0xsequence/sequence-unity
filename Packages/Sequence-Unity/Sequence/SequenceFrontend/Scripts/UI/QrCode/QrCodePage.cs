using System;
using Sequence.Marketplace;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    [RequireComponent(typeof(QrCodeView))]
    public class QrCodePage : WalletAddressCopyPage
    {
        private QrCodeView _qrCodeView;
        private QrCodeParams _qrCodeParams;

        protected override void Awake()
        {
            base.Awake();
            _qrCodeView = GetComponent<QrCodeView>();
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            _qrCodeParams = args.GetObjectOfTypeIfExists<QrCodeParams>();
            if (_qrCodeParams == null)
            {
                throw new ArgumentException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(QrCodeParams)} as an argument");
            }

            _qrCodeView.Show(_qrCodeParams).ConfigureAwait(false);
        }
    }
}