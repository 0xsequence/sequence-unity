using TMPro;
using UnityEngine;

namespace Sequence.Boilerplates
{
    public class SessionWalletTile : MonoBehaviour
    {
        [SerializeField] private TMP_Text _addressText;
        
        private Address _signer;
        
        public void Apply(Address signer)
        {
            _signer = signer;
            _addressText.text = $"{_signer}";
        }
    }
}
