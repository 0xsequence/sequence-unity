using System;
using System.Collections;
using Sequence.Demo.ScriptableObjects;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class TokenInfoPage : UIPage
    {
        [SerializeField] private Image _tokenIconImage;
        [SerializeField] private TextMeshProUGUI _tokenNameText;
        [SerializeField] private Image _networkIconImage;
        [SerializeField] private TextMeshProUGUI _networkNameText;
        [SerializeField] private TextMeshProUGUI _balanceText;
        [SerializeField] private TextMeshProUGUI _currencyValueText;
        public float TimeBetweenTokenValueRefreshesInSeconds = 5;
        private NetworkIcons _networkIconsMapper;
        
        private TokenElement _tokenElement;
        private BalanceCurrencyTextSetter _balanceCurrencyTextSetter;

        public override void Open(params object[] args)
        {
            base.Open(args);
            TokenElement tokenElement = args.GetObjectOfTypeIfExists<TokenElement>();
            if (tokenElement == default)
            {
                throw new SystemException(
                    $"Invalid use. {nameof(TokenInfoPage)} must be opened with a {typeof(TokenElement)} as an argument");
            }
            NetworkIcons networkIcons = args.GetObjectOfTypeIfExists<NetworkIcons>();
            if (tokenElement == default)
            {
                throw new SystemException(
                    $"Invalid use. {nameof(TokenInfoPage)} must be opened with a {typeof(NetworkIcons)} as an argument");
            }

            _tokenElement = tokenElement;
            _networkIconsMapper = networkIcons;
            Assemble();
            StartCoroutine(RefreshTokenValueRepeatedly());
        }

        private void Assemble()
        {
            _tokenIconImage.sprite = _tokenElement.TokenIconSprite;
            _tokenNameText.text = _tokenElement.TokenName;
            _networkIconImage.sprite = _networkIconsMapper.GetIcon(_tokenElement.Network);
            _networkNameText.text = ChainNames.NameOf[_tokenElement.Network];

            _balanceCurrencyTextSetter = new BalanceCurrencyTextSetter(_balanceText, _currencyValueText, _tokenElement);
            _balanceCurrencyTextSetter.SetInitialValueAndBalanceText();
        }

        public void RefreshCurrencyValue()
        {
            ThrowIfNotAssembled();
            _balanceCurrencyTextSetter.RefreshCurrencyValue();
        }
        
        public void RefreshWithBalance(uint balance)
        {
            ThrowIfNotAssembled();
            _balanceCurrencyTextSetter.RefreshWithBalance(balance);
        }

        private void ThrowIfNotAssembled()
        {
            if (_balanceCurrencyTextSetter == null)
            {
                throw new SystemException(
                    $"{typeof(TokenInfoPage)} must be assembled via {nameof(Assemble)} before use.");
            }
        }

        private IEnumerator RefreshTokenValueRepeatedly()
        {
            var waitForRefresh = new WaitForSecondsRealtime(TimeBetweenTokenValueRefreshesInSeconds);
            while (true) // Terminates on Close() (as this gameObject will be disabled)
            {
                yield return waitForRefresh;
                RefreshCurrencyValue();
            }
        }

        public Sprite GetNetworkIcon(Chain network)
        {
            return _networkIconsMapper.GetIcon(network);
        }
    }
}