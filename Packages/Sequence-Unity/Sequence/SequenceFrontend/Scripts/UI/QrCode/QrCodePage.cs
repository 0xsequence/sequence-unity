using System;
using Sequence.Marketplace;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    [RequireComponent(typeof(LegacyQrCodeView))]
    public class QrCodePage : WalletAddressCopyPage
    {
        private LegacyQrCodeView _legacyQrCodeView;
        private QrCodeParams _qrCodeParams;

        protected override void Awake()
        {
            base.Awake();
            _legacyQrCodeView = GetComponent<LegacyQrCodeView>();
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

            _legacyQrCodeView.Show(_qrCodeParams).ConfigureAwait(false);
        }
    }
}