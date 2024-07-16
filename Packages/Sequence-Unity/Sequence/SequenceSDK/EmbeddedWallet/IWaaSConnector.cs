using System.Threading.Tasks;
using Sequence.Authentication;

namespace Sequence.EmbeddedWallet
{
    public interface IWaaSConnector
    {
        public Task<string> InitiateAuth(IntentDataInitiateAuth initiateAuthIntent, LoginMethod method);
        public Task ConnectToWaaS(IntentDataOpenSession loginIntent, LoginMethod method, string email = "");
        public Task FederateAccount(IntentDataFederateAccount federateAccountIntent, LoginMethod method, string email);
    }
}