using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.EmbeddedWallet;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Sequence.Demo
{
    public class SequenceInGameShop : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private Chain _chain = Chain.TestnetArbitrumSepolia;
        [SerializeField] private string _tokenContractAddress;
        [SerializeField] private string _saleContractAddress;
        [SerializeField] private int[] _itemsForSale;
        
        [Header("Components")]
        [SerializeField] private GameObject _loadingView;
        [SerializeField] private GameObject _resultView;
        [SerializeField] private QrCodeView _qrCodeView;
        [SerializeField] private TMP_Text _resultText;
        [SerializeField] private TMP_Text _paymentTokenText;
        [SerializeField] private TMP_Text _userAddressText;
        [SerializeField] private TMP_Text _userBalanceText;
        [SerializeField] private TMP_Text _supplyText;
        [SerializeField] private TMP_Text _startTimeText;
        [SerializeField] private TMP_Text _endTimeText;
        
        [Header("Tile Object Pool")]
        [SerializeField] private GenericObjectPool<SequenceInGameShopTile> _tilePool;

        private IWallet _wallet;
        private SequenceInGameShopState _saleState;

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show(IWallet wallet)
        {
            _wallet = wallet;
            
            gameObject.SetActive(true);
            _loadingView.SetActive(false);
            _resultView.SetActive(false);
            _qrCodeView.gameObject.SetActive(false);
            
            Assert.IsNotNull(_wallet, "Could not get a SequenceWallet reference from the UIPage.Open() arguments.");
            
            RefreshState();
        }

        public async void OpenQrCodeView()
        {
            var destinationAddress = _wallet.GetWalletAddress();
            await _qrCodeView.Show(_saleState.PaymentToken, (int)_chain, destinationAddress, "1e2");
        }

        public async void RefreshState()
        {
            ClearState();

            _saleState = new SequenceInGameShopState();

            SetLoading(true);
            await _saleState.Construct(
                new Address(_saleContractAddress), 
                new Address(_tokenContractAddress), 
                _wallet, 
                _chain,
                _itemsForSale);

            SetLoading(false);
            RenderState();
        }

        private void RenderState()
        {
            _paymentTokenText.text = _saleState.PaymentTokenSymbol;
            _userAddressText.text= _wallet.GetWalletAddress();
            _userBalanceText.text= $"{_saleState.UserPaymentBalance} {_saleState.PaymentTokenSymbol}";
            _supplyText.text = $"{_saleState.TotalMinted}/{_saleState.SupplyCap}";
            _startTimeText.text = ConvertTime(_saleState.StartTime);
            _endTimeText.text = ConvertTime(_saleState.EndTime);
            
            LoadTiles();
        }

        private void ClearState()
        {
            _paymentTokenText.text = string.Empty;
            _userAddressText.text= string.Empty;
            _userBalanceText.text= string.Empty;
            _supplyText.text = string.Empty;
            _startTimeText.text = string.Empty;
            _endTimeText.text = string.Empty;
            _tilePool.Cleanup();
        }
        
        private void LoadTiles()
        {
            _tilePool.Cleanup();
            foreach (var (tokenId, supply) in _saleState.TokenSupplies)
            {
                var tileObject = _tilePool.GetObject();
                tileObject.Initialize(
                    tokenId, 
                    supply.tokenMetadata, 
                    _saleState.Cost,
                    _saleState.PaymentTokenSymbol, 
                    supply.supply, 
                    PurchaseToken);
            }
        }

        private async Task PurchaseToken(BigInteger tokenId, int amount)
        {
            SetLoading(true);
            var success = await _saleState.PurchaseAsync(tokenId, amount);
            SetLoading(false);
            SetResult(success);
            
            if (success)
                RenderState();
            else
                OpenQrCodeView();
        }

        private void SetLoading(bool loading)
        {
            _loadingView.SetActive(loading);
        }

        private void SetResult(bool success)
        {
            _resultView.SetActive(true);
            _resultText.text = success ? "Success" : "Failed";
        }

        private string ConvertTime(int timestamp)
        {
            var localDateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
            return localDateTime.ToString("dd.MM.yyyy HH:mm:ss");
        }
    }
}