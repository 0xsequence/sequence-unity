using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.EmbeddedWallet;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Sequence.Demo
{
    public class PrimarySalePage : UIPage
    {
        [Serializable]
        private struct SaleContractConfig
        {
            [field: SerializeField] public SaleContractType Type { get; private set; }
            [field: SerializeField] public Chain Chain { get; private set; }
            [field: SerializeField] public string TokenContractAddress { get; private set; }
            [field: SerializeField] public string SaleContractAddress { get; private set; }
        }
        
        [Header("Configuration")] 
        [SerializeField] private SaleContractType _saleContractType;
        [SerializeField] private SaleContractConfig[] _configs;
        
        [Header("Components")]
        [SerializeField] private QrCodeView _qrCodeView;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _paymentTokenText;
        [SerializeField] private TMP_Text _userAddressText;
        [SerializeField] private TMP_Text _userBalanceText;
        [SerializeField] private TMP_Text _supplyText;
        [SerializeField] private TMP_Text _startTimeText;
        [SerializeField] private TMP_Text _endTimeText;
        
        [Header("Tile Object Pool")]
        [SerializeField] private Transform _marketplaceTileParent;
        [SerializeField] private GameObject _marketplaceTilePrefab;
        [SerializeField] private int _objectPoolAmount = 10;

        private SaleContractConfig _config;
        private SequenceWallet _wallet;
        private IPrimarySaleState _saleState;
        private ObjectPool _tilePool;

        public override void Open(params object[] args)
        {
            base.Open(args);
            _qrCodeView.gameObject.SetActive(false);
            
            _wallet = args.GetObjectOfTypeIfExists<SequenceWallet>();
            Assert.IsNotNull(_wallet);

            _config = Array.Find(_configs, c => c.Type == _saleContractType);
            
            _saleState = _saleContractType switch
            {
                SaleContractType.ERC721 => new PrimarySaleStateERC721(),
                SaleContractType.ERC1155 => new PrimarySaleStateERC1155(),
                _ => null
            };
            
            Assert.IsNotNull(_saleState);
            RefreshState();
        }

        public async void OpenQrCodeView()
        {
            var destinationAddress = _wallet.GetWalletAddress();
            await _qrCodeView.Show((int)_config.Chain, destinationAddress, "1e2");
        }

        public async void RefreshState()
        {
            await _saleState.Construct(
                _config.SaleContractAddress, 
                _config.TokenContractAddress, 
                _wallet, 
                _config.Chain);
            
            RenderState();
        }

        private void RenderState()
        {
            _titleText.text = $"Primary Sales Demo ({_saleContractType})";
            _paymentTokenText.text = _saleState.PaymentTokenSymbol;
            _userAddressText.text= _wallet.GetWalletAddress();
            _userBalanceText.text= $"{_saleState.UserPaymentBalance} {_saleState.PaymentTokenSymbol}";
            _supplyText.text = $"{_saleState.TotalMinted}/{_saleState.SupplyCap}";
            _startTimeText.text = ConvertTime(_saleState.StartTime);
            _endTimeText.text = ConvertTime(_saleState.EndTime);
            
            LoadTiles();
        }
        
        private void LoadTiles()
        {
            _tilePool?.Cleanup();
            _tilePool = ObjectPool.ActivateObjectPool(
                _marketplaceTilePrefab, 
                _objectPoolAmount, 
                true, 
                _marketplaceTileParent);
            
            foreach (var (tokenId, supply) in _saleState.TokenSupplies)
            {
                var tileObject = _tilePool.GetNextAvailable().GetComponent<PrimarySaleTile>();
                tileObject.Initialize(tokenId, supply.tokenMetadata, _saleState.Cost, _saleState.PaymentTokenSymbol, 
                    supply.supply, PurchaseToken);
            }
        }

        private async Task PurchaseToken(BigInteger tokenId, int amount)
        {
            var success = await _saleState.Purchase(tokenId, amount);
            if (success)
                RenderState();
            else
                OpenQrCodeView();
        }

        private string ConvertTime(int timestamp)
        {
            var localDateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
            return localDateTime.ToString("dd.MM.yyyy HH:mm:ss");
        }
    }
}