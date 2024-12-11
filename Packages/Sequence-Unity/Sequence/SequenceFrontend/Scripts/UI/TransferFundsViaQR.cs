using Sequence.EmbeddedWallet;
using UnityEngine;
using UnityEngine.UI;
using Sequence.Marketplace;

namespace Sequence.Demo
{
    [RequireComponent(typeof(Button))]
    public class TransferFundsViaQR : MonoBehaviour, ICheckoutOption
    {
        [SerializeField] private GameObject _qrPanel;
        [SerializeField] private QrCodeView _qrCodeView;

        private CollectibleOrder _order;
        private IWallet _wallet;
        
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Checkout);
        }
        
        public async void Checkout()
        {
            if (_order != null)
            {
                await _qrCodeView.Show("0x00000000000000000000000000000000", _order.order.chainId, 
                    "0x00000000000000000000000000000000", "1e2"); 
                _qrPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("Collectible order not set for checkout.");
            }
        }

        public void SetWallet(SequenceWallet wallet)
        {
            _wallet = wallet;
        }

        public void SetCollectibleOrder(CollectibleOrder checkoutOrder)
        {
            _order = checkoutOrder;
        }
    
        public void Close()
        {
            _qrPanel.SetActive(false);
        }
    }
}
