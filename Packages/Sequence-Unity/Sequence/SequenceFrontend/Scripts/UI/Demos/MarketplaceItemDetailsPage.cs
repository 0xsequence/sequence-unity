using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sequence.Marketplace;

namespace Sequence.Demo
{
    public class MarketplaceItemDetailsPage : UIPage
    {
        [SerializeField] UIPanel _getCheckoutOptionsPanel;
        [SerializeField] Image _image;
        [SerializeField] private TextMeshProUGUI _nameText, _priceText, _descriptionText, _attributesText;
        CollectibleOrder _order;

        protected override void Awake()
        {
            base.Awake();
        }
        public override void Open(params object[] args)
        {
            ClearPage();

            base.Open(args);

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
            _priceText.text = string.Empty;
            _descriptionText.text = string.Empty;
            _attributesText.text = string.Empty;
            _order = null;
        }

        async void FillPage()
        {
            var texture = await UnityWebRequestExtensions.DownloadImage(_order.metadata.image);
            _image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            _nameText.text = _order.metadata.name;
            _priceText.text = _order.order.priceUSD.ToString();
            _descriptionText.text = _order.metadata.description;
            _attributesText.text = _order.metadata.attributes[0].Values.ToString();
        }
        public void Checkout()
        {
            _getCheckoutOptionsPanel.Open(_order);
        }
    }
}