using System;
using System.Threading.Tasks;
using Sequence.Contracts;
using Sequence.Marketplace;
using Sequence.Provider;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class CartItem : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _amountField;
        [SerializeField] private Image _collectibleImage;
        [SerializeField] private TextMeshProUGUI _collectionNameText;
        [SerializeField] private TextMeshProUGUI _collectibleNameText;
         
        private CollectibleOrder _order;
        private uint _amountRequested;
        private Sprite _collectibleSprite;
        private string _collectionName;
        private string _collectibleName;
        private ICheckoutHelper _cart;

        public void Assemble(ICheckoutHelper cart, string orderId, string collectionName = "")
        {
            _cart = cart;
            _order = _cart.GetListings().GetCollectibleOrder(orderId);
            _collectibleSprite = _cart.GetCollectibleImagesByOrderId()[orderId];
            _amountRequested = _cart.GetAmountsRequestedByOrderId()[orderId];
            _collectionName = collectionName;
            _collectibleName = _order.metadata.name;

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
            Chain chain = ChainDictionaries.ChainById[_order.order.chainId.ToString()];
            IEthClient client = new SequenceEthClient(chain);
            Contract contract = new Contract(_order.order.collectionContractAddress);

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
                    _collectionName = collectionName;
                    _collectionNameText.text = _collectionName;
                    _collectionNameText.gameObject.SetActive(true);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to get collection name from contract {_order.order.collectionContractAddress}: {e.Message}. No collection name will be displayed in the UI. If the collection name is known, please provide it when calling {nameof(Assemble)}");
            }
        }

        public void IncrementAmount()
        {
            if (uint.TryParse(_order.order.quantityAvailable, out uint quantityAvailable))
            {
                if (_amountRequested < quantityAvailable)
                {
                    _amountRequested++;
                    _amountField.text = _amountRequested.ToString();
                    _cart.SetAmountRequested(_order.order.orderId, _amountRequested);
                }
            }
            else
            {
                throw new SystemException($"Failed to parse quantity available for {_order.order.orderId}, given: {_order.order.quantityAvailable}");
            }
        }
        
        public void DecrementAmount()
        {
            if (_amountRequested > 0)
            {
                _amountRequested--;
                _amountField.text = _amountRequested.ToString();
                _cart.SetAmountRequested(_order.order.orderId, _amountRequested);
            }
        }
        
        private void OnAmountFieldChanged(string value)
        {
            if (uint.TryParse(value, out uint newValue))
            {
                if (newValue > 0)
                {
                    _amountRequested = newValue;
                }
                else
                {
                    _amountRequested = 0;
                }
                if (newValue > uint.Parse(_order.order.quantityAvailable))
                {
                    _amountRequested = uint.Parse(_order.order.quantityAvailable);
                }
                _cart.SetAmountRequested(_order.order.orderId, _amountRequested);
                _amountField.text = _amountRequested.ToString();
            }
            else
            {
                _amountField.text = _amountRequested.ToString();
            }
        }
    }
}