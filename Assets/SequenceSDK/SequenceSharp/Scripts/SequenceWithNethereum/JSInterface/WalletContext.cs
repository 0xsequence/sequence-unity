namespace SequenceSharp
{
    public class WalletContext
    {
#nullable enable
        public string factory;
        public string mainModule;
        public string mainModuleUpgradable;
        public string? guestModule;
        public string? sequenceUtils;
        public WalletContextLibs? libs;
        public bool? nonStrict;
#nullable disable

        public WalletContext(string factory, string mainModule, string mainModuleUpgradable)
        {
            this.factory = factory;
            this.mainModule = mainModule;
            this.mainModuleUpgradable = mainModuleUpgradable;
        }

    }

    public class WalletContextLibs
    {
#nullable enable
        public string? requireFreshSigner;
#nullable disable
    }
}