using System;
using System.Threading.Tasks;

namespace Sequence.EcosystemWallet.Authentication
{
    public class SequenceEcosystemWallet
    {
        public Address Address { get; }
        
        public SequenceEcosystemWallet(Address address)
        {
            Address = address;   
        }

        public async Task<string> SignMessage(string message)
        {
            throw new NotImplementedException();
        }
        
        public async Task SendTransaction()
        {
            throw new NotImplementedException();
        }
    }
}