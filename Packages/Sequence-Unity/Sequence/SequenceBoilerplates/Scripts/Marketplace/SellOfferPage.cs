using System;
using TMPro;
using System.Threading.Tasks;
using Sequence.Utils;
using Sequence.Marketplace;
using UnityEngine;

namespace Sequence.Boilerplates
{
    public class SellOfferPage : MonoBehaviour
    {  
        [SerializeField] private GameObject _loadingScreen;
          
        private CollectibleOrder _offer;
        private ICheckout _checkout;

        [SerializeField] TextMeshProUGUI _name, _quantityAvailable;

        int _amount;

        public void Open(params object[] args)
        {
            _offer = args.GetObjectOfTypeIfExists<CollectibleOrder>();
            if (_offer == null)
            {
                throw new ArgumentException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(CollectibleOrder)} as an argument");
            }

        }

        public void SetAmount(string value)
        {
            if (int.TryParse(value, out int parsedAmount)) // Parse input safely
            {
                if (int.TryParse(_offer.order.quantityAvailable, out int availableQuantity)) // Parse quantityAvailable safely
                {
                    // Ensure _amount does not exceed availableQuantity
                    _amount = (parsedAmount > availableQuantity) ? availableQuantity : parsedAmount;
                }
                else
                {
                    Debug.LogError("Invalid quantityAvailable format."); // Handle invalid quantityAvailable
                }
            }
        }

        public void SellOffer()
        {
            Sell().ConfigureAwait(false);
        }

        private async Task Sell()
        {
            _loadingScreen.SetActive(true);

            Step[] steps = await _checkout.GenerateSellTransaction(_offer, _amount);
            _loadingScreen.SetActive(false);
        }
    }
}
