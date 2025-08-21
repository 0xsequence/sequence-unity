using UnityEngine;

namespace Sequence.Utils
{
    public class UrlSchemeFactory
    {
        public static string CreateFromAppIdentifier()
        {
            return Application.identifier.Replace(".", "").ToLower();
        }
    }
}