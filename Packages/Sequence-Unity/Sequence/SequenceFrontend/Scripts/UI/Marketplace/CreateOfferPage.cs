using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Threading.Tasks;
using Sequence.Utils;
using Sequence.Marketplace;
using UnityEngine;

namespace Sequence.Demo
{
    public class CreateOfferPage : UIPage
    {
        [SerializeField] private GameObject _loadingScreen;

        private TokenBalance _nft;
        private Chain _chain;
        private ICheckout _checkout;

        int _price, _amount;

        [SerializeField] TMP_Dropdown _currencyPicker;

        Address _currencyTokenAddress;

        DateTime expiryDate;
        

        protected override void Awake()
        {
            base.Awake();
        }

        public override void Open(params object[] args)
        {
            base.Open(args);

            _nft = args.GetObjectOfTypeIfExists<TokenBalance>();
            if (_nft == null)
            {
                throw new ArgumentException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(TokenBalance)} as an argument");
            }

            Configure();

        }

        protected void Configure()
        {
            _chain = ChainDictionaries.ChainById[_nft.chainId.ToString()];

            // Clear and populate the dropdown
            _currencyPicker.ClearOptions();
            var options = new List<string>(ChainDictionaries.NativeTokenAddressOf.Keys.Select(chain => chain.ToString()));
            _currencyPicker.AddOptions(options);

            // Set default currency if available
            if (ChainDictionaries.NativeTokenAddressOf.TryGetValue(_chain, out string defaultAddress))
            {
                _currencyTokenAddress = new Address(defaultAddress);
                _currencyPicker.value = options.IndexOf(_chain.ToString());
            }

            // Add listener for dropdown selection change
            _currencyPicker.onValueChanged.AddListener(OnCurrencySelected);
        }

        private void OnCurrencySelected(int index)
        {
            var selectedChain = ChainDictionaries.NativeTokenAddressOf.Keys.ElementAt(index);
            if (ChainDictionaries.NativeTokenAddressOf.TryGetValue(selectedChain, out string tokenAddress))
            {
                _currencyTokenAddress = new Address(tokenAddress);
            }
        }

        public override void Close()
        {
            base.Close();

        }

        public void SetPrice(string value)
        {
            if (int.TryParse(value, out int parsedPrice))
            {
                _price = parsedPrice;
            }
        }

        public void SetQuantity(string value)
        {
            if (int.TryParse(value, out int parsedQuantity))
            {
                _amount = parsedQuantity;
            }
        }

        public void SetListingCurrency()
        {

        }
        public void SetDate()
        {

        }

        public void ListItem()
        {
            List().ConfigureAwait(false);
        }

        protected async Task List()
        {
            _loadingScreen.SetActive(true);

            Step[] steps = await _checkout.GenerateOfferTransaction(new Address(_nft.contractAddress), _nft.tokenID.ToString(), _amount, ContractTypeExtensions.AsMarketplaceContractType(_nft.contractType), _currencyTokenAddress, _price, expiryDate);

            _loadingScreen.SetActive(false);
            if (0 == 0)
            {
                _panel.Close();
            }
            else
            {
            }
        }


    }
}