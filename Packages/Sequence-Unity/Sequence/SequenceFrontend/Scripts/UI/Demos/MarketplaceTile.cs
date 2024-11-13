using System;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using Sequence.Marketplace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class MarketplaceTile : MonoBehaviour
    {
        private Image _image;
        [SerializeField ]private TextMeshProUGUI _nameText, _priceText;
        private CollectibleOrder _collectibleOrder;
        private Button _interactButton;
        private void Awake()
        {
            _image = GetComponent<Image>();
            _interactButton = GetComponent<Button>();
            _interactButton.onClick.AddListener(() => SequenceSampleUI.instance.OpenViewMarketplaceDetailsPanelWithDelay(0, _collectibleOrder));
        }

        public void Assemble(CollectibleOrder order)
        {
            _collectibleOrder = order;
            FetchImage().ConfigureAwait(false);
            _nameText.text = new string(_collectibleOrder.metadata.name);
            _priceText.text = "US"+ _collectibleOrder.order.priceUSD.ToString("C6", new CultureInfo("en-US"));
        }

        private async Task FetchImage()
        {
            gameObject.SetActive(false);
            _image.sprite = await SpriteFetcher.Fetch(_collectibleOrder.metadata.image);
            gameObject.SetActive(true);
        }
    }
}