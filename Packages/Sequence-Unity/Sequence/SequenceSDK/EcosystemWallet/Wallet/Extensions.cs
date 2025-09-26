namespace Sequence.EcosystemWallet
{
    public struct Extensions
    {
        public Address Sessions;
        public Address Guard;
    }

    public static class ExtensionsFactory
    {
        public static Extensions Current => Rc3;
        
        public static Extensions Rc3 = new Extensions
        {
            Sessions = new Address("0x0000000000CC58810c33F3a0D78aA1Ed80FaDcD8"),
            Guard = new Address("0x18002Fc09deF9A47437cc64e270843dE094f5984")
        };
    }
}