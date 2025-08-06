using Sequence.Wallet;

namespace Sequence.EcosystemWallet
{
    public class SessionSigner
    {
        public Address ParentAddress { get; }
        public Address Address { get; }
        public Chain Chain { get; }
        public bool IsExplicit { get; }

        private readonly SessionCredentials _credentials;
        
        internal SessionSigner(SessionCredentials credentials)
        {
            _credentials = credentials;

            ParentAddress = credentials.address;
            Address = new EOAWallet(credentials.privateKey).GetAddress();
            Chain = ChainDictionaries.ChainById[credentials.chainId];
            IsExplicit = credentials.isExplicit;
        }
    }
}