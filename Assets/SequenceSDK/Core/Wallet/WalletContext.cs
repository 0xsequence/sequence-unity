namespace Sequence.Core.Wallet {
    public class WalletContext
    {
        public string FactoryAddress { get; set; }
        public string MainModuleAddress { get; set; }
        public string MainModuleUpgradableAddress { get; set; }
        public string GuestModuleAddress { get; set; }
        public string UtilsAddress { get; set; }
    }
}