using System.Collections.Generic;

namespace Sequence.EcosystemWallet
{
    internal static class EcosystemBindings
    {
        private static Dictionary<EcosystemType, string> UrlBindings = new ()
        {
            { EcosystemType.Sequence, "https://v3.sequence-dev.app" }
        };
        
        public static string GetUrl(EcosystemType ecosystem)
        {
            return UrlBindings[ecosystem];
        }
    }
}