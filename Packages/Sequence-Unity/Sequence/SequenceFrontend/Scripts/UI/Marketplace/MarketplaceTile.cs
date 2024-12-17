using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using NBitcoin.RPC;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Sequence.Demo
{
    [RequireComponent(typeof(Button))]
    public class MarketplaceTile : MonoBehaviour
    {
        public static event Action<CollectibleOrder> OnCollectibleSelected;
        
        [SerializeField] private Image _collectibleImage, _currencyIcon;
        [SerializeField] private TextMeshProUGUI _nameText, _priceText, _amountAvailableText;
        [SerializeField] private GameObject _seeInfoButton;
        [SerializeField] private GameObject _buyButton;
        private CollectibleOrder _collectibleOrder;
        private Button _focusButton;
        private MarketplaceItemDetailsPage _marketplaceItemDetailsPage;
        private IWallet _wallet;
        private CheckoutPanel _checkoutPanel;
        private Sprite _collectibleSprite;
        
        private void Awake()
        {
            _focusButton = GetComponent<Button>();
            _focusButton.onClick.AddListener(() =>
            {
                OnCollectibleSelected?.Invoke(_collectibleOrder);
            });
            OnCollectibleSelected += HandleCollectibleSelected;
        }
        
        private void OnDestroy()
        {
            OnCollectibleSelected -= HandleCollectibleSelected;
        }

        public void Assemble(CollectibleOrder order, Sprite currencyIcon, IWallet wallet, CheckoutPanel checkoutPanel)
        {
            _collectibleOrder = order;
            FetchImage().ConfigureAwait(false);
            _currencyIcon.sprite = currencyIcon;
            _nameText.text = new string(_collectibleOrder.metadata.name);
            _priceText.text = _collectibleOrder.order.priceAmountFormatted;
            _amountAvailableText.text = "Available: " + _collectibleOrder.order.quantityAvailable;
            _wallet = wallet;
            _checkoutPanel = checkoutPanel;
        }

        private async Task FetchImage()
        {
            gameObject.SetActive(false);
            _collectibleSprite = await AssetHandler.GetSpriteAsync(_collectibleOrder.metadata.image);
            _collectibleImage.sprite = _collectibleSprite;
            gameObject.SetActive(true);
        }

        private void HandleCollectibleSelected(CollectibleOrder order)
        {
            if (order == null)
            {
                ResetButtons();
                return;
            }
            
            if (order.order.orderId == _collectibleOrder.order.orderId)
            {
                ToggleButtons();
            }
            else
            {
                ResetButtons();
            }
        }
        
        private void ToggleButtons()
        {
            _focusButton.interactable = false;
            if (_seeInfoButton != null)
            {
                _seeInfoButton.SetActive(true);
            }

            if (_buyButton != null)
            {
                _buyButton.SetActive(true);
            }
        }

        private void ResetButtons()
        {
            _focusButton.interactable = true;
            if (_seeInfoButton != null)
            {
                _seeInfoButton.SetActive(false);
            }
            if (_buyButton != null)
            {
                _buyButton.SetActive(false);
            }
        }

        public void OpenMarketplaceItemDetailsPage()
        {
            if (_marketplaceItemDetailsPage == null)
            {
                _marketplaceItemDetailsPage = FindObjectOfType<MarketplaceItemDetailsPage>();
            }

            _marketplaceItemDetailsPage.Open(_collectibleOrder);
        }

        public void OpenBuyPage()
        {
            _checkoutPanel.Open(new NftCheckout(_wallet, _collectibleOrder, _collectibleSprite, 1));
        }
    }
}