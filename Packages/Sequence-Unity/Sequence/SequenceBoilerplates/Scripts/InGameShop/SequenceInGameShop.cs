using System.Collections;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Demo.Utils;
using Sequence.EmbeddedWallet;
using SequenceSDK.Samples;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Sequence.Demo
{
    public class SequenceInGameShop : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject _loadingView;
        [SerializeField] private QrCodeView _qrCodeView;
        [SerializeField] private TMP_Text _endTimeText;
        [SerializeField] private MessagePopup _messagePopup;
        
        [Header("Tile Object Pool")]
        [SerializeField] private GenericObjectPool<SequenceInGameShopTile> _tilePool;

        private IWallet _wallet;
        private Chain _chain;
        private string _tokenContractAddress;
        private string _saleContractAddress;
        private int[] _itemsForSale;
        private SequenceInGameShopState _saleState;

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show(IWallet wallet, Chain chain, string tokenContractAddress, string saleContractAddress, int[] itemsForSale)
        {
            _wallet = wallet;
            _chain = chain;
            _tokenContractAddress = tokenContractAddress;
            _saleContractAddress = saleContractAddress;
            _itemsForSale = itemsForSale;
            
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