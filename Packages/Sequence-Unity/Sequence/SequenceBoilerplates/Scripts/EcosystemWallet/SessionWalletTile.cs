using System;
using Sequence.EcosystemWallet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Boilerplates
{
    public class SessionWalletTile : MonoBehaviour
    {
        [SerializeField] private TMP_Text _addressText;
        [SerializeField] private Button _removeButton;
        
        private SequenceSessionWallet _wallet;
        private Action<Address> _onRemove;
        
        public void Apply(SequenceSessionWallet wallet, Action<Address> onRemove)
        {
            _wallet = wallet;
            _onRemove = onRemove;
            _addressText.text = $"({(wallet.IsExplicit ? "E" : "I")}) {wallet.Address}";
            _removeButton.gameObject.SetActive(wallet.IsExplicit);
        }

        public void Remove()
        {
            _onRemove?.Invoke(_wallet.Address);
        }
    }
}
