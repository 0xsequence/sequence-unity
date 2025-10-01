namespace Sequence.EcosystemWallet
{
    public struct Extensions
    {
        public Address Factory;
        public Address Stage1;
        public Address Stage2;
        public string CreationCode;
        public Address Sessions;
        public Address Guard;
    }

    public static class ExtensionsFactory
    {
        public static Extensions Rc3 = new Extensions
        {
            Factory = new Address("0x00000000000018A77519fcCCa060c2537c9D6d3F"),
            Stage1 = new Address("0x00000000000084fA81809Dd337311297C5594d62"),
            Stage2 = new Address("0x7438718F9E4b9B834e305A620EEeCf2B9E6eBE79"),
            CreationCode = "0x6041600e3d396021805130553df33d3d36153402601f57363d3d373d363d30545af43d82803e903d91601f57fd5bf3",
            Sessions = new Address("0x0000000000CC58810c33F3a0D78aA1Ed80FaDcD8"),
            Guard = new Address("0x18002Fc09deF9A47437cc64e270843dE094f5984")
        };
        
        public static Extensions Current => Rc3;
    }
}