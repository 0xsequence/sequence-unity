using Sequence.EmbeddedWallet;
using TMPro;
using UnityEngine;

namespace Sequence.Boilerplates
{
    public class VersionText : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        private SequenceWallet _wallet;

        private void Awake()
        {
            SequenceWallet.OnWalletCreated += OnWalletCreated;
        }

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            OnWalletCreated(null);
        }

        private void OnDestroy()
        {
            SequenceWallet.OnWalletCreated -= OnWalletCreated;
        }

        private void OnWalletCreated(SequenceWallet wallet)
        {
            _wallet = wallet;
            var versionFile = Resources.Load<TextAsset>("sequence-unity-version");
            _text.text = $"Sequence Unity SDK {versionFile.text}";
        }
    }
}
