using System;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using Sequence.Marketplace;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class MarketplaceTile : MonoBehaviour
    {
        [SerializeField] private Image _collectibleImage, _currencyIcon;
        [SerializeField] private TextMeshProUGUI _nameText, _priceText, _amountAvailableText;
        private CollectibleOrder _collectibleOrder;
        private Button _interactButton;
        private void Awake()
        {
            _interactButton = GetComponent<Button>();
            _interactButton.onClick.AddListener(() => SequenceSampleUI.instance.OpenViewMarketplaceDetailsPanelWithDelay(0, _collectibleOrder));
        }

        public void Assemble(CollectibleOrder order, Sprite currencyIcon)
        {
            _collectibleOrder = order;
            FetchImage().ConfigureAwait(false);
            _currencyIcon.sprite = currencyIcon;
            _nameText.text = new string(_collectibleOrder.metadata.name);
            _priceText.text = _collectibleOrder.order.priceAmountFormatted;
            _amountAvailableText.text = "Available: " + _collectibleOrder.order.quantityAvailable;
        }

        private async Task FetchImage()
        {
            gameObject.SetActive(false);
            _collectibleImage.sprite = await SpriteFetcher.Fetch(_collectibleOrder.metadata.image);
            gameObject.SetActive(true);
        }
    }
}