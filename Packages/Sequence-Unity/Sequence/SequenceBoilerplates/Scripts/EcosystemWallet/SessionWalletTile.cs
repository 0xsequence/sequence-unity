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
        
        private Address _signer;
        private Action<Address> _onRemove;
        
        public void Apply(Address signer, Action<Address> onRemove)
        {
            _signer = signer;
            _onRemove = onRemove;
            _addressText.text = $"{_signer}";
        }

        public void Remove()
        {
            _onRemove?.Invoke(_signer);
        }
    }
}
