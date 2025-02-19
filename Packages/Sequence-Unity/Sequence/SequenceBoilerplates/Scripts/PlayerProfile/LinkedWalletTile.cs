using Sequence.EmbeddedWallet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceSDK.Samples
{
    public class LinkedWalletTile : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Button _removeButton;
        
        public void Show(LinkedWalletData wallet)
        {
            _nameText.text = wallet.linkedWalletAddress;
            _removeButton.gameObject.SetActive(true);
        }

        public void ShowEmpty()
        {
            _nameText.text = "You don't have any wallets linked.";
            _removeButton.gameObject.SetActive(false);
        }
    }
}