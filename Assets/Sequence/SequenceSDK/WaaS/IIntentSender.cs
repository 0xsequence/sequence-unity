using System.Threading.Tasks;
using Sequence.WaaS.Authentication;
using SequenceSDK.WaaS;

namespace Sequence.WaaS
{
    public interface IIntentSender
    {
        public Task<T> SendIntent<T, T2>(T2 args, IntentType type, uint timeBeforeExpiryInSeconds = 30, uint currentTime = 0);
        public Task<bool> DropSession(string dropSessionId);
        public Task<T> PostIntent<T>(string payload, string path);
        public Task<WaaSSession[]> ListSessions();
        public Task<SuccessfulTransactionReturn> GetTransactionReceipt(SuccessfulTransactionReturn response);
    }
}