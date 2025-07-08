using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Boilerplates
{
    public class NetworkBanner : MonoBehaviour
    {
        [SerializeField] private Image _networkIcon;
        [SerializeField] private TextMeshProUGUI _networkName;
        [SerializeField] private NetworkIcons _networkIcons;
        
        private Chain _chain;

        public void Assemble(Chain chain)
        {
            _chain = chain;
            _networkIcon.sprite = _networkIcons.GetIcon(chain);
            _networkName.text = ChainDictionaries.NameOf[chain];
        }
    }
}