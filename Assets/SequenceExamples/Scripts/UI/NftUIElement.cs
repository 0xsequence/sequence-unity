using Sequence.Demo.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class NftUIElement : MonoBehaviour
    {
        [SerializeField] private Image _nftImage;
        
        public NetworkIcons NetworkIcons;
        public ITransactionDetailsFetcher TransactionDetailsFetcher = new MockTransactionDetailsFetcher(15); // Todo: replace mock with concrete implementation
        private NftElement _nftElement;
        private WalletPanel _walletPanel;

        public void Assemble(NftElement nftElement)
        {
            _nftElement = nftElement;

            _nftImage.sprite = nftElement.TokenIconSprite;
        }

        public void SwitchToInfoPage()
        {
            if (_walletPanel == null)
            {
                _walletPanel = FindObjectOfType<WalletPanel>();
            }
            
            _walletPanel.OpenNftInfoPage(_nftElement, NetworkIcons, TransactionDetailsFetcher);
        }
    }
}