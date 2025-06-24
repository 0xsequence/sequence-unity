using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using TMPro;
using Sequence.Marketplace;
using Sequence.Utils;

namespace Sequence.Demo
{
    public class MarketplaceItemDetailsPage : MonoBehaviour
    {
        [SerializeField] CheckoutPage _getCheckoutOptionsPanel;
        [SerializeField] Image _image;
        [SerializeField] private TextMeshProUGUI _nameText, _usdPriceText, _priceText, _descriptionText, _attributesText;
        CollectibleOrder _order;
        
        public void Open(params object[] args)
        {
            ClearPage();

            foreach (var item in args)
            {
                if (item is CollectibleOrder order) _order = order;
                break;
            }
            
            FillPage();
        }

        void ClearPage()
        {
            _image.sprite = null;
            _nameText.text = string.Empty;
            _usdPriceText.text = string.Empty;
            _priceText.text = string.Empty;
            _descriptionText.text = string.Empty;
            _attributesText.text = string.Empty;
            _order = null;
        }

        async void FillPage()
        {
            _image.sprite = await AssetHandler.GetSpriteAsync(_order.metadata.image);
            _nameText.text = new string(_order.metadata.name);
            _usdPriceText.text = "US" + _order.order.priceUSD.ToString("C6", new CultureInfo("en-US"));
            _priceText.text = _order.order.priceAmountFormatted;
            _descriptionText.text = _order.metadata.description;
            _attributesText.text = _order.metadata.attributes[0].Values.ToString();
        }
        
        public void Checkout()
        {
            _getCheckoutOptionsPanel.Open(_order);
        }
    }
}