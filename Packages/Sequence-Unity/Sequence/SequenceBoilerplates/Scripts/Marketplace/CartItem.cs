using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Contracts;
using Sequence.Marketplace;
using Sequence.Provider;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Boilerplates
{
    public class CartItem : MonoBehaviour
    {
        public static event Action<BigInteger> OnAmountChanged;
        
        [SerializeField] private TMP_InputField _amountField;
        [SerializeField] private Image _collectibleImage;
        [SerializeField] private TextMeshProUGUI _collectionNameText;
        [SerializeField] private TextMeshProUGUI _collectibleNameText;
        [SerializeField] private Button _incrementAmountButton;
         
        private CartItemData _itemData;
        private ulong _amountRequested;
        private Sprite _collectibleSprite;
        private string _collectionName;
        private string _collectibleName;
        private ICheckoutHelper _cart;
        private Address _collectionAddress;
        private string _tokenId;

        public void Assemble(ICheckoutHelper cart, CartItemData itemData, string collectionName = "")
        {
            _cart = cart;
            _itemData = itemData;
            _collectionAddress = itemData.Collection;
            _tokenId = itemData.TokenId;
            _collectibleSprite = _cart.GetCollectibleImagesByCollectible()[_collectionAddress][_tokenId];
            _amountRequested = _cart.GetAmountsRequestedByCollectible()[_collectionAddress][_tokenId];
            _collectionName = collectionName;
            _collectibleName = itemData.Name;

            if (string.IsNullOrEmpty(_collectionName))
            {
                TryToGetCollectionNameFromContract().ConfigureAwait(false);
            }
            
            _amountField.text = _amountRequested.ToString();
            _amountField.onValueChanged.AddListener(OnAmountFieldChanged);
            _collectibleImage.sprite = _collectibleSprite;
            _collectionNameText.text = _collectionName;
            _collectibleNameText.text = _collectibleName;
        }
        
        private async Task TryToGetCollectionNameFromContract()
        {
            Chain chain = _itemData.Network;
            IEthClient client = new SequenceEthClient(chain);
            Contract contract = new Contract(_itemData.Collection);

            try
            {
                string collectionName = await contract.SendQuery<string>(client, "name()");
                if (string.IsNullOrWhiteSpace(collectionName) || collectionName == "0x")
                {
                    _collectionName = "Unknown";
                    _collectionNameText.gameObject.SetActive(false);
                }
                else
                {
                    _collectionName = collectionName.HexStringToHumanReadable();
                    _collectionNameText.text = _collectionName;
                    _collectionNameText.gameObject.SetActive(true);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to get collection name from contract {_itemData.Collection}: {e.Message}. No collection name will be displayed in the UI. If the collection name is known, please provide it when calling {nameof(Assemble)}");
            }
        }

        public void IncrementAmount()
        {
            SetAmount(_amountRequested + 1).ConfigureAwait(false);
        }

        private async Task SetAmount(ulong newAmount)
        {
            ulong remaining = await _cart.SetAmountRequested(_collectionAddress, _tokenId, _amountRequested);
            _incrementAmountButton.interactable = remaining == 0;
            _amountRequested = newAmount - remaining;
            _amountField.text = _amountRequested.ToString();
            OnAmountChanged?.Invoke(newAmount);
        }

        public void DecrementAmount()
        {
            if (_amountRequested > 0)
            {
                SetAmount(_amountRequested - 1).ConfigureAwait(false);
            }
        }
        
        private void OnAmountFieldChanged(string value)
        {
            if (ulong.TryParse(value, out ulong newValue))
            {
                SetAmount(newValue).ConfigureAwait(false);
            }
            else
            {
                Debug.LogWarning("Unable to parse provided value: " + value + " as a number");
                _amountField.text = _amountRequested.ToString();
            }
        }
    }
}