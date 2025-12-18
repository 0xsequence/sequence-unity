using System.Numerics;

namespace Sequence.EcosystemWallet
{
    public enum EcosystemAuthProvider
    {
        Email,
        Google,
        Apple,
        Passkey,
        Mnemonic,
        Unknown
    }
    
    public struct EcosystemConfig
    {
        public string name;
        public string description;
        public string url;
        public BigInteger[] supportedChains;
        public EcosystemAuthProvider[] enabledProviders;
    }
}