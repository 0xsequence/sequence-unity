namespace Sequence.EcosystemWallet
{
    public struct Extensions
    {
        public Address Sessions;
    }

    public static class ExtensionsFactory
    {
        public static Extensions Rc3 = new Extensions
        {
            Sessions = new Address("0x0000000000CC58810c33F3a0D78aA1Ed80FaDcD8")
        };
    }
}