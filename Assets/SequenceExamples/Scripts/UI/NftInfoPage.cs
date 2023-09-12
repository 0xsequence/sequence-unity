using System;
using System.Collections;
using System.Collections.Generic;
using Sequence.Demo.ScriptableObjects;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class NftInfoPage : PageWithTransactionDetailsBlocks
    {
        [SerializeField] private Image _collectionIcon;
        [SerializeField] private TextExtender _collectionNameText;
        [SerializeField] private Image _networkIcon;
        [SerializeField] private TextMeshProUGUI _nftNameText;
        [SerializeField] private TextMeshProUGUI _nftNumberText;
        [SerializeField] private Image _nftImage;
        [SerializeField] private TextExtender _ethValueText;
        [SerializeField] private TextMeshProUGUI _currencyValueText;
        public float TimeBetweenEthValueRefreshesInSeconds = 5;

        private NftElement _nftElement;
        private AmountAndCurrencyTextSetter _amountAndCurrencyTextSetter;
        
        private RectTransform _scrollRectContent;
        private VerticalLayoutGroup _verticalLayoutGroup;
        
        protected override void Awake()
        {
            base.Awake();
            _scrollRectContent = GetComponentInChildren<ScrollRect>().content;
            _verticalLayoutGroup = _scrollRectContent.GetComponent<VerticalLayoutGroup>();
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            NftElement nftElement = args.GetObjectOfTypeIfExists<NftElement>();
            if (nftElement == default)
            {
                throw new SystemException(
                    $"Invalid use. {nameof(NftInfoPage)} must be opened with a {typeof(NftElement)} as an argument");
            }

            _nftElement = nftElement;
            
            Assemble();
            StartCoroutine(RefreshEthValueRepeatedly());
        }

        private void Assemble()
        {
            _collectionIcon.sprite = _nftElement.CollectionIconSprite;
            _collectionNameText.SetText(_nftElement.CollectionName, true);
            _networkIcon.sprite = _networkIcons.GetIcon(_nftElement.Network);
            _nftNameText.text = _nftElement.TokenName;
            _nftNumberText.text = $"#{_nftElement.TokenNumber}";
            _nftImage.sprite = _nftElement.TokenIconSprite;
            
            _amountAndCurrencyTextSetter = new AmountAndCurrencyTextSetter(_ethValueText, _currencyValueText, _nftElement);
            _amountAndCurrencyTextSetter.SetInitialValueAndAmountText();
        }

        private IEnumerator RefreshEthValueRepeatedly()
        {
            var waitForRefresh = new WaitForSecondsRealtime(TimeBetweenEthValueRefreshesInSeconds);
            while (true) // Terminates on Close() (as this gameObject will be disabled)
            {
                yield return waitForRefresh;
                RefreshCurrencyValue();
            }
        }

        public void RefreshCurrencyValue()
        {
            ThrowIfNotAssembled();
            _amountAndCurrencyTextSetter.RefreshCurrencyValue();
        }
        
        public void RefreshWithBalance(uint balance)
        {
            ThrowIfNotAssembled();
            _amountAndCurrencyTextSetter.RefreshWithAmount(balance);
        }

        private void ThrowIfNotAssembled()
        {
            if (_amountAndCurrencyTextSetter == null)
            {
                throw new SystemException(
                    $"{typeof(NftInfoPage)} must be assembled via {nameof(Assemble)} before use.");
            }
        }

        public Sprite GetNetworkIcon(Chain network)
        {
            return _networkIcons.GetIcon(network);
        }

        protected override void UpdateScrollViewSize()
        {
            base.UpdateScrollViewSize();
            float contentHeight = _verticalLayoutGroup.preferredHeight;
            _scrollRectContent.sizeDelta = new Vector2(_scrollRectContent.sizeDelta.x, contentHeight);
        }
    }
}