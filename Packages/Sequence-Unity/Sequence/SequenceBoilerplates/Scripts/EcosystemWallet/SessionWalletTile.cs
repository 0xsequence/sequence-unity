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
        
        private SessionSigner _signer;
        private Action<Address> _onRemove;
        
        public void Apply(SessionSigner signer, Action<Address> onRemove)
        {
            _signer = signer;
            _onRemove = onRemove;
            _addressText.text = $"({(signer.IsExplicit ? "E" : "I")}) {signer.Address}";
            _removeButton.gameObject.SetActive(signer.IsExplicit);
        }

        public void Remove()
        {
            _onRemove?.Invoke(_signer.Address);
        }
    }
}
