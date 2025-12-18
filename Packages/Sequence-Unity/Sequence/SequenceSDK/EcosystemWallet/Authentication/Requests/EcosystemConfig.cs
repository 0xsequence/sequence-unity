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
        public EcosystemAuthProvider[] enabledProviders;
    }
}