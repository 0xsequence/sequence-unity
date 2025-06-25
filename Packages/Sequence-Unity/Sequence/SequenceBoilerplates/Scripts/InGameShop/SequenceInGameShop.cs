using System;
using System.Collections;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Contracts;
using Sequence.Demo;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using Sequence.Pay;
using Sequence.Provider;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

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

        private IWallet _wallet;
        private Chain _chain;
        private string _tokenContractAddress;
        private string _saleContractAddress;
        private int[] _itemsForSale;
        private Action _onClose;
        private SequenceInGameShopState _saleState;

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
        public void Show(IWallet wallet, Chain chain, string tokenContractAddress, string saleContractAddress, 
            int[] itemsForSale, Action onClose = null)
        {
            _wallet = wallet;
            _chain = chain;
            _tokenContractAddress = tokenContractAddress;
            _saleContractAddress = saleContractAddress;
            _itemsForSale = itemsForSale;
            _onClose = onClose;
            
            gameObject.SetActive(true);
            _loadingView.SetActive(false);
            _messagePopup.gameObject.SetActive(false);
            _qrCodeView.gameObject.SetActive(false);
            
            Assert.IsNotNull(_wallet, "Could not get a SequenceWallet reference from the UIPage.Open() arguments.");
            
            RefreshState();
        }

        public async void OpenQrCodeView()
        {
            _qrCodeView.gameObject.SetActive(true);
            var destinationAddress = _wallet.GetWalletAddress();
            await _qrCodeView.Show(_saleState.PaymentToken, destinationAddress, "1e2");
        }

        public void OpenInventory()
        {
            SetLoading(true);
            BoilerplateFactory.OpenSequenceInventory(transform.parent, _wallet, _chain, 
                new [] {_tokenContractAddress}, () => SetLoading(false));
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
            ERC1155 collection = new ERC1155(_tokenContractAddress);
            string uri = await collection.URI(new SequenceEthClient(_chain), tokenId);
            Sprite collectibleImage = await AssetHandler.GetSpriteAsync(uri);
            ERC1155Sale sale = new ERC1155Sale(_saleContractAddress);
            
            ICheckoutHelper checkoutHelper = await ERC1155SaleCheckout.Create(sale,
                new ERC1155(_tokenContractAddress), tokenId.ToString(), amount, _chain, _wallet,
                "Demo Primary Sale Checkout", _paymentTokenIconUrl, collectibleImage);
            
            BoilerplateFactory.OpenCheckoutPanel(transform.parent, _chain, checkoutHelper,
                new SequenceCheckout(_wallet, _chain, sale, collection, tokenId.ToString(), amount));
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