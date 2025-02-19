using System;
using System.Numerics;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Boilerplates.InGameShop
{
    public class SequenceInGameShopTile : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private Button _purchaseButton;

        private BigInteger _cost;
        private string _symbol;
        private BigInteger _tokenId;
        private int _amount;
        private Func<BigInteger, int, Task> _purchase;
        
        public async void Initialize(BigInteger tokenId, TokenMetadata metadata, BigInteger cost, string symbol, 
            string supply, Func<BigInteger, int, Task> purchase)
        {
            _tokenId = tokenId;
            _cost = cost;
            _symbol = symbol;
            _purchase = purchase;
            _nameText.text = $"{metadata?.name}";
            SetAmount(0);
            
            _image.sprite = await AssetHandler.GetSpriteAsync(metadata?.image);
        }

        public async void OnPurchaseClicked()
        {
            _purchaseButton.interactable = false;
            await _purchase.Invoke(_tokenId, _amount);
            _purchaseButton.interactable = true;
            
            SetAmount(0);
        }

        public void UpdateAmount(int addition)
        {
            SetAmount(_amount + addition);
        }

        private void SetAmount(int amount)
        {
            _amount = Mathf.Max(0, amount);
            _amountText.text = $"{_amount}";
            _priceText.text = $"Purchase for {_cost} {_symbol}";
            _purchaseButton.interactable = _amount > 0;
        }
    }
}