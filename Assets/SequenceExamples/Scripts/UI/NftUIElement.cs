using Sequence.Demo.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class NftUIElement : WalletUIElement
    {
        [SerializeField] private Image _nftImage;
        
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

        public override Chain GetNetwork()
        {
            return _nftElement.Collection.Network;
        }

        public CollectionInfo GetCollection()
        {
            return _nftElement.Collection;
        }
    }
}