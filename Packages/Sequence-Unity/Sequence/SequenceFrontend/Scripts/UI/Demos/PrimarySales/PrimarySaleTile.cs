using System;
using System.Numerics;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class PrimarySaleTile : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private TMP_Text _supplyText;
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private Button _purchaseButton;

        private BigInteger _tokenId;
        private int _amount;
        private Func<BigInteger, int, Task> _purchase;
        
        public async void Initialize(BigInteger tokenId, TokenMetadata metadata, BigInteger cost, string symbol, 
            string supply, Func<BigInteger, int, Task> purchase)
        {
            _tokenId = tokenId;
            _purchase = purchase;
            _image.sprite = await AssetHandler.GetSpriteAsync(metadata?.image);
            _nameText.text = $"{metadata?.name} (#{tokenId})";
            _priceText.text = $"Price: {cost} {symbol}";
            _supplyText.text = $"Total Minted: {supply}";
            SetAmount(0);
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
            _amountText.text = $"{_amount}x";
            _purchaseButton.interactable = _amount > 0;
        }
    }
}