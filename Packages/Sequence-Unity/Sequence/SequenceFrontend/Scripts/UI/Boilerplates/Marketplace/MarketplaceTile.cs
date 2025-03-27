using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using NBitcoin.RPC;
using Sequence.Boilerplates;
using Sequence.Demo;
using Sequence.Demo.Mocks;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using Sequence.Pay;
using Sequence.Pay.Sardine;
using Sequence.Pay.Transak;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Sequence.Boilerplates.Marketplace
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
        private Sprite _collectibleSprite;
        private NFTType _nftType;
        private BoilerplateController _boilerplateController;
        
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

        public void Assemble(CollectibleOrder order, Sprite currencyIcon, IWallet wallet, CheckoutPanel checkoutPanel, NFTType nftType)
        {
            if (order == null)
            {
                throw new ArgumentNullException($"{nameof(order)} cannot be null");
            }

            if (!String.Equals(order.order.priceCurrencyAddress, "0x3c499c542cEF5E3811e1192ce70d8cC03d5c3359", StringComparison.InvariantCultureIgnoreCase))
            {
                Destroy(gameObject);
                return;
            }
            _collectibleOrder = order;
            FetchImage().ConfigureAwait(false);
            _currencyIcon.sprite = currencyIcon;
            _nameText.text = new string(_collectibleOrder.metadata.name);
            _priceText.text = _collectibleOrder.order.priceAmountFormatted;
            _amountAvailableText.text = "Available: " + _collectibleOrder.order.quantityAvailable;
            _wallet = wallet;
            _nftType = nftType;
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
            if (_boilerplateController == null)
            {
                _boilerplateController = FindObjectOfType<BoilerplateController>();
            }
            _boilerplateController.OpenCheckoutPanel(new NftCheckout(_wallet, _collectibleOrder, _collectibleSprite, 1),
                new SequenceCheckout(_wallet, ChainDictionaries.ChainById[_collectibleOrder.order.chainId.ToString()],
                    _collectibleOrder , 1, _nftType,
                    pay: new SequencePay(_wallet,
                        ChainDictionaries.ChainById[_collectibleOrder.order.chainId.ToString()],
                        checkout: new MockCheckoutWithSardineForCanada(new Checkout(_wallet,
                            ChainDictionaries.ChainById[_collectibleOrder.order.chainId.ToString()])))));
        }
    }
}