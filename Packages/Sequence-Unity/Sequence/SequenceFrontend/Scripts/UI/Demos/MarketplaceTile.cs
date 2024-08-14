using System;
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
        private TextMeshProUGUI _priceText;
        private CollectibleOrder _collectibleOrder;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _priceText = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Assemble(CollectibleOrder order)
        {
            _collectibleOrder = order;
            FetchImage().ConfigureAwait(false);
            _priceText.text = order.order.priceUSD.ToString("C6", new CultureInfo("en-US"));
        }

        private async Task FetchImage()
        {
            gameObject.SetActive(false);
            Sprite sprite = await SpriteFetcher.Fetch(_collectibleOrder.metadata.image);
            _image.sprite = sprite;
            gameObject.SetActive(true);
        }
    }
}