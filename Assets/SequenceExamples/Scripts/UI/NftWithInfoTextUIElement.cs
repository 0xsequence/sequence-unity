using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class NftWithInfoTextUIElement : WalletUIElement
    {
        [SerializeField] private Image _nftImage;
        [SerializeField] private TextMeshProUGUI _nftNameText;
        [SerializeField] private TextMeshProUGUI _numberOwnedText;

        private NftElement _nftElement;
        private WalletPanel _walletPanel;

        public void Assemble(NftElement nftElement, WalletPanel panel)
        {
            _nftElement = nftElement;

            _nftImage.sprite = nftElement.TokenIconSprite;
            _nftNameText.text = _nftElement.TokenName;
            _numberOwnedText.text = $"{_nftElement.Balance} Owned";

            _walletPanel = panel;
        }

        public void SwitchToInfoPage()
        {
            _walletPanel.OpenNftInfoPage(_nftElement, NetworkIcons, TransactionDetailsFetcher);
        }

        public override Chain GetNetwork()
        {
            return _nftElement.Collection.Network;
        }
    }
}