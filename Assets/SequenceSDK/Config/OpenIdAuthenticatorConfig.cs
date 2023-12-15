using UnityEngine;

namespace Sequence.Config
{
    [CreateAssetMenu(fileName = "OpenIdAuthenticatorConfig", menuName = "Sequence/OpenIdAuthenticatorConfig", order = 1)]
    public class OpenIdAuthenticatorConfig : ScriptableObject
    {
        [Header("URL Scheme Configuration")]
        public string UrlScheme = "sdk-powered-by-sequence";
    }
}