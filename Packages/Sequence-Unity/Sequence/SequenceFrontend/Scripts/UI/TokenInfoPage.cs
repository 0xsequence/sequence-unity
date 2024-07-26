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
    public class TokenInfoPage : InfoPage
    {
        [SerializeField] private Image _tokenIconImage;
        [SerializeField] private TextMeshProUGUI _tokenNameText;
        [SerializeField] private Image _networkIconImage;
        [SerializeField] private TextMeshProUGUI _networkNameText;
        
        private TokenElement _tokenElement;
        private AmountAndCurrencyTextSetter _amountAndCurrencyTextSetter;

        public override void Open(params object[] args)
        {
            base.Open(args);
            TokenElement tokenElement = args.GetObjectOfTypeIfExists<TokenElement>();
            if (tokenElement == default)
            {
                throw new SystemException(
                    $"Invalid use. {nameof(TokenInfoPage)} must be opened with a {typeof(TokenElement)} as an argument");
            }

            _tokenElement = tokenElement;
            
            Assemble();
        }

        private void Assemble()
        {
            _tokenIconImage.sprite = _tokenElement.TokenIconSprite;
            _tokenNameText.text = _tokenElement.TokenName;
            _networkIconImage.sprite = _networkIcons.GetIcon(_tokenElement.Network);
            _networkNameText.text = ChainDictionaries.NameOf[_tokenElement.Network];
        }
    }
}