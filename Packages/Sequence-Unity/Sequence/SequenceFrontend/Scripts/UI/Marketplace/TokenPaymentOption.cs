using System;
using System.Threading.Tasks;
using Sequence.Marketplace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    [RequireComponent(typeof(Button))]
    public class TokenPaymentOption : MonoBehaviour
    {
        public static event Action<Marketplace.Currency> OnSelectedCurrency;
        
        [SerializeField] private Image _checkbox;
        [SerializeField] private Sprite _uncheckedSprite;
        [SerializeField] private Sprite _checkedSprite;
        [SerializeField] private TextMeshProUGUI _tokenNameText;
        [SerializeField] private TextMeshProUGUI _tokenAmountText;
        [SerializeField] private Image _tokenIcon;

        private Marketplace.Currency _currency;
        private Cart _cart;

        private void Awake()
        {
            OnSelectedCurrency += HandleCurrencySelected;
        }
        
        private void OnDestroy()
        {
            OnSelectedCurrency -= HandleCurrencySelected;
        }

        public void Assemble(Marketplace.Currency currency, string amount, Sprite tokenIcon = null)
        {
            _currency = currency;
            _tokenNameText.text = currency.name;
            _tokenAmountText.text = $"{amount} {currency.symbol}";
            
            if (tokenIcon != null)
            {
                _tokenIcon.sprite = tokenIcon;
            }
            else
            {
                FetchAndApplyTokenIconSprite().ConfigureAwait(false);
            }
            
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private async Task FetchAndApplyTokenIconSprite()
        {
            _tokenIcon.sprite = await AssetHandler.GetSpriteAsync(_currency.imageUrl);
        }

        private void OnClick()
        {
            OnSelectedCurrency?.Invoke(_currency);
        }

        private void HandleCurrencySelected(Marketplace.Currency selectedCurrency)
        {
            if (selectedCurrency == _currency)
            {
                _checkbox.sprite = _checkedSprite;
            }
            else
            {
                _checkbox.sprite = _uncheckedSprite;
            }
        }
    }
}