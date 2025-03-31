using System;
using System.Globalization;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using System.Threading.Tasks;
using Sequence.Utils;
using Sequence.Marketplace;
using UnityEngine;

namespace Sequence.Demo
{
    public class ListItemPage : UIPage
    {

        [SerializeField] DatePicker _datePicker;
        [SerializeField] GameObject _loadingScreen;

        private TokenBalance _nft;
        private Chain _chain;
        private ICheckout _checkout;

        int _price, _amount;

        [SerializeField] TMP_Dropdown _currencyPicker;

        Marketplace.Currency[] _currencies;
        Address _currencyTokenAddress;


        protected override void Awake()
        {
            base.Awake();
        }

        public override void Open(params object[] args)
        {
            base.Open(args);

            _checkout = args.GetObjectOfTypeIfExists<ICheckout>();
            if (_checkout == null)
            {
                throw new ArgumentException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(ICheckout)} as an argument");
            }

            _nft = args.GetObjectOfTypeIfExists<TokenBalance>();
            if (_nft == null)
            {
                throw new ArgumentException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(TokenBalance)} as an argument");
            }

            Configure();

        }

        protected async void Configure()
        {
            _chain = ChainDictionaries.ChainById[_nft.chainId.ToString()];
            _currencyPicker.ClearOptions();

            var marketplaceReader = new MarketplaceReader(_chain);
            _currencies = await marketplaceReader.ListCurrencies();

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            foreach (var item in _currencies)
            {
                options.Add(new TMP_Dropdown.OptionData(item.symbol)); 
            }

            _currencyPicker.AddOptions(options); 
            _currencyTokenAddress = new Address(_currencies[0].contractAddress);
            _currencyPicker.onValueChanged.AddListener(OnCurrencySelected);
        }


        private void OnCurrencySelected(int index)
        {
            _currencyTokenAddress = new Address(_currencies[index].contractAddress); // Assign correct address        
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

        public void ListItem()
        {
            List().ConfigureAwait(false);
        }

        protected async Task List()
        {
            _loadingScreen.SetActive(true);

            DateTime date = DateTime.ParseExact(
                _datePicker.Date.ToString("MM/dd/yyyy"),
                "MM/dd/yyyy",
                CultureInfo.InvariantCulture
            );

            Step[] steps = await _checkout.GenerateListingTransaction(new Address(_nft.contractAddress), _nft.tokenID.ToString() , new BigInteger(_amount), ContractTypeExtensions.AsMarketplaceContractType(_nft.contractType), _currencyTokenAddress, new BigInteger(_price), date);

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
