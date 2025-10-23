using System;
using System.Collections;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Adapter;
using Sequence.Contracts;
using Sequence.Marketplace;
using Sequence.Pay;
using Sequence.Provider;
using TMPro;
using UnityEngine;

namespace Sequence.Boilerplates.InGameShop
{
    public class SequenceInGameShop : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject _loadingView;
        [SerializeField] private QrCodeView _qrCodeView;
        [SerializeField] private TMP_Text _endTimeText;
        [SerializeField] private MessagePopup _messagePopup;

        [SerializeField] private string _paymentTokenIconUrl =
            "https://cryptologos.cc/logos/usd-coin-usdc-logo.png";
        
        [Header("Tile Object Pool")]
        [SerializeField] private GenericObjectPool<SequenceInGameShopTile> _tilePool;

        private EmbeddedWalletAdapter _adapter;
        
        private string _tokenContractAddress;
        private string _saleContractAddress;
        private int[] _itemsForSale;
        private Action _onClose;
        private SequenceInGameShopState _saleState;
        
        private void Awake()
        {
            _adapter = EmbeddedWalletAdapter.GetInstance();
        }

        /// <summary>
        /// This function is called when the user clicks the close button.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            _onClose?.Invoke();
        }

        /// <summary>
        /// Required function to configure this Boilerplate.
        /// </summary>
        /// <param name="wallet">This Wallet instance will perform transactions.</param>
        /// <param name="chain">Chain used to get balances and send transactions.</param>
        /// <param name="tokenContractAddress">ERC1155 Contract you deployed on Sequence's Builder.</param>
        /// <param name="saleContractAddress">ERC1155 Sale Contract you deployed on Sequence's Builder.</param>
        /// <param name="itemsForSale">Define the token Ids you want to sell from your collection.</param>
        /// <param name="onClose">(Optional) Callback when the user closes this window.</param>
        public void Show(string tokenContractAddress, string saleContractAddress, 
            int[] itemsForSale, Action onClose = null)
        {
            _tokenContractAddress = tokenContractAddress;
            _saleContractAddress = saleContractAddress;
            _itemsForSale = itemsForSale;
            _onClose = onClose;
            
            gameObject.SetActive(true);
            _loadingView.SetActive(false);
            _messagePopup.gameObject.SetActive(false);
            _qrCodeView.gameObject.SetActive(false);
            
            RefreshState();
        }

        public async void OpenQrCodeView()
        {
            _qrCodeView.gameObject.SetActive(true);
            var destinationAddress = _adapter.Wallet.GetWalletAddress();
            await _qrCodeView.Show(_saleState.PaymentToken, destinationAddress, "1e2");
        }

        public void OpenInventory()
        {
            SetLoading(true);

            var collections = new[] { _tokenContractAddress };
            BoilerplateFactory.OpenSequenceInventory(transform.parent, collections, () => SetLoading(false));
        }
        
        public async void RefreshState()
        {
            ClearState();

            _saleState = new SequenceInGameShopState();

            SetLoading(true);
            await _saleState.Construct(
                new Address(_saleContractAddress), 
                new Address(_tokenContractAddress), 
                _itemsForSale);

            SetLoading(false);
            RenderState();
        }

        private void RenderState()
        {
            LoadTiles();
            StopAllCoroutines();
            StartCoroutine(TimerRoutine());
        }

        private void ClearState()
        {
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
                    PurchaseToken, 
                    AdditionalCheckoutOptions);
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

        private async Task AdditionalCheckoutOptions(BigInteger tokenId, ulong amount)
        {
            var chain = _adapter.Chain;
            ERC1155 collection = new ERC1155(_tokenContractAddress);
            string uri = await collection.URI(new SequenceEthClient(chain), tokenId);
            Sprite collectibleImage = await AssetHandler.GetSpriteAsync(uri);
            ERC1155Sale sale = new ERC1155Sale(_saleContractAddress);
            
            ICheckoutHelper checkoutHelper = await ERC1155SaleCheckout.Create(sale,
                new ERC1155(_tokenContractAddress), tokenId.ToString(), amount, chain, _adapter.Wallet,
                "Demo Primary Sale Checkout", _paymentTokenIconUrl, collectibleImage);
            
            BoilerplateFactory.OpenCheckoutPanel(transform.parent, chain, checkoutHelper,
                new SequenceCheckout(_adapter.Wallet, chain, sale, collection, tokenId.ToString(), amount));
        }

        private void SetLoading(bool loading)
        {
            _loadingView.SetActive(loading);
        }

        private void SetResult(bool success)
        {
            _messagePopup.Show(success ? "Success" : "Failed");
        }

        private IEnumerator TimerRoutine()
        {
            while (true)
            {
                var remainingTime = TimeUtils.FormatRemainingTime(_saleState.EndTime - TimeUtils.GetTimestampSecondsNow());
                _endTimeText.text = $"Ends in {remainingTime}";
                yield return new WaitForSeconds(1f);
            }
        }
    }
}