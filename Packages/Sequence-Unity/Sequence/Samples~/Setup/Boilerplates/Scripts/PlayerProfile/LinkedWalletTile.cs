using System;
using Sequence.EmbeddedWallet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Boilerplates.PlayerProfile
{
    public class LinkedWalletTile : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Button _removeButton;

        private Action _unlink;
        
        public void Show(LinkedWalletData wallet, Action unlink)
        {
            _unlink = unlink;
            _nameText.text = wallet.linkedWalletAddress;
            _removeButton.gameObject.SetActive(true);
        }

        public void ShowEmpty()
        {
            _nameText.text = "You don't have any wallets linked.";
            _removeButton.gameObject.SetActive(false);
        }

        public void UnlinkWallet()
        {
            _unlink?.Invoke();
        }
    }
}