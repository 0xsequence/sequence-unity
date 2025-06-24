using System;
using System.Collections.Generic;
using System.Numerics;
using System.Globalization;
using TMPro;
using System.Threading.Tasks;
using Sequence.Utils;
using Sequence.Marketplace;
using UnityEngine;

namespace Sequence.Demo
{
    public class CreateOfferPage : MonoBehaviour
    {
        [SerializeField] DatePicker _datePicker;
        [SerializeField] GameObject _loadingScreen;

        private TokenBalance _nft;
        private CollectibleOrder _order;

        Marketplace.Currency[] _currencies;
        private ICheckout _checkout;

        int _price, _amount;

        [SerializeField] TMP_Dropdown _currencyPicker;

        Address _currencyTokenAddress;        

        public void Open(params object[] args)
        {
            _nft = args.GetObjectOfTypeIfExists<TokenBalance>();
            if (_nft == null)
            {
                _order = args.GetObjectOfTypeIfExists<CollectibleOrder>();
                if (_order == null)
                {
                    throw new ArgumentException(
                        $"Invalid use. {GetType().Name} must be opened with a {typeof(TokenBalance)} or {typeof(CollectibleOrder)} as an argument");
                }  
            }

            Configure();

        }

        protected async void Configure()
        {
            Chain chain;
            if (_nft != null) chain = ChainDictionaries.ChainById[_nft.chainId.ToString()];
            else if (_order != null) chain = ChainDictionaries.ChainById[_order.order.chainId.ToString()];
            else return;

            _currencyPicker.ClearOptions();

            var marketplaceReader = new MarketplaceReader(chain);
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

            if (_nft != null)
            {
                Step[] steps = await _checkout.GenerateOfferTransaction(new Address(_nft.contractAddress), _nft.tokenID.ToString(), new BigInteger(_amount), ContractTypeExtensions.AsMarketplaceContractType(_nft.contractType), _currencyTokenAddress, new BigInteger(_price), date);
            }
            else if (_order != null)
            {
                var response = await Indexer.GetTokenSupplies(_order.order.chainId, new GetTokenSuppliesArgs(_order.order.collectionContractAddress));
                var type = response.contractType;

                Step[] steps = await _checkout.GenerateOfferTransaction(new Address(_order.order.collectionContractAddress), _order.metadata.tokenId.ToString(), new BigInteger(_amount), ContractTypeExtensions.AsMarketplaceContractType(type), _currencyTokenAddress, new BigInteger(_price), date);

            }
            _loadingScreen.SetActive(false);
        }
    }
}