using Sequence.Authentication;
using Sequence.Config;
using Sequence.EmbeddedWallet;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class EmailSignedInText : MonoBehaviour
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
            var sdkVersion = SequenceConfig.GetConfig(SequenceService.WaaS).WaaSVersion;
            var email = PlayerPrefs.GetString(OpenIdAuthenticator.LoginEmail);
            
            if (_wallet == null || string.IsNullOrEmpty(email))
                _text.text = sdkVersion;
            else
                _text.text = $"Logged in as: {email}, {sdkVersion}";
        }
    }
}
