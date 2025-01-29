using Sequence;
using Sequence.Demo;
using Sequence.EmbeddedWallet;
using TMPro;
using UnityEngine;

namespace SequenceSDK.Samples
{
    public class SequencePlayerProfile : MonoBehaviour
    {
        [Header("Components")] 
        [SerializeField] private Chain _chain = Chain.TestnetArbitrumSepolia;
        
        [Header("Components")]
        [SerializeField] private TMP_Text _walletAddressText;
        [SerializeField] private TMP_Text _etherBalanceText;
        [SerializeField] private QrCodeView _qrImage;
        [SerializeField] private GameObject _backButton;
        [SerializeField] private GameObject _overviewState;
        [SerializeField] private GameObject _sendTokenState;
        
        private IWallet _wallet;

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public async void Show(IWallet wallet)
        {
            _wallet = wallet;
            
            gameObject.SetActive(true);
            SetOverviewState(true);
            
            var walletAddress = _wallet.GetWalletAddress();
            _walletAddressText.text = walletAddress;
            
            await _qrImage.Show("", (int)_chain, walletAddress, "1e2");
            var balance = await new ChainIndexer(_chain).GetEtherBalance(walletAddress);
            _etherBalanceText.text = balance.ToString();
        }

        public void CopyWalletAddress()
        {
            
        }

        public async void SignOut()
        {
            await _wallet.DropThisSession();
            Hide();
        }

        public void SetOverviewState(bool overview)
        {
            _backButton.SetActive(!overview);
            _overviewState.SetActive(overview);
            _sendTokenState.SetActive(!overview);
        }
    }
}
