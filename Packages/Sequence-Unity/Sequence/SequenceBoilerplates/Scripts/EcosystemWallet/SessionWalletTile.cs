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
        
        public void Apply(Address signer)
        {
            _signer = signer;
            _addressText.text = $"{_signer}";
        }
    }
}
