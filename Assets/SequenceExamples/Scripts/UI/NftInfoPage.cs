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
    public class NftInfoPage : InfoPage
    {
        [SerializeField] private Image _collectionIcon;
        [SerializeField] private TextExtender _collectionNameText;
        [SerializeField] private Image _networkIcon;
        [SerializeField] private TextMeshProUGUI _nftNameText;
        [SerializeField] private TextMeshProUGUI _nftNumberText;
        [SerializeField] private Image _nftImage;

        private NftElement _nftElement;
        
        private RectTransform _scrollRectContent;
        private VerticalLayoutGroup _verticalLayoutGroup;
        
        private WalletPanel _walletPanel;
        
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
        }

        private void Assemble()
        {
            _collectionIcon.sprite = _nftElement.Collection.IconSprite;
            _collectionNameText.SetText(_nftElement.Collection.Name, true);
            _networkIcon.sprite = _networkIcons.GetIcon(_nftElement.Collection.Network);
            _nftNameText.text = _nftElement.TokenName;
            _nftNumberText.text = $"#{_nftElement.TokenNumber}";
            _nftImage.sprite = _nftElement.TokenIconSprite;
        }

        protected override void UpdateScrollViewSize()
        {
            base.UpdateScrollViewSize();
            float contentHeight = _verticalLayoutGroup.preferredHeight;
            _scrollRectContent.sizeDelta = new Vector2(_scrollRectContent.sizeDelta.x, contentHeight);
        }

        public void SwitchToCollectionInfoPage()
        {
            if (_walletPanel == null)
            {
                _walletPanel = FindObjectOfType<WalletPanel>();
            }
            
            _walletPanel.OpenCollectionInfoPage(_networkIcons, _nftElement.Collection);
        }
    }
}