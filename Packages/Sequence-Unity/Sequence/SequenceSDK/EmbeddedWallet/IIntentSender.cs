using System.Threading.Tasks;

namespace Sequence.EmbeddedWallet
{
    public interface IIntentSender
    {
        public Task<T> SendIntent<T, T2>(T2 args, IntentType type, uint timeBeforeExpiryInSeconds = 30);
        public Task<bool> DropSession(string dropSessionId);
        public Task<T> PostIntent<T>(string payload, string path);
        public Task<WaaSSession[]> ListSessions();
        public Task<SuccessfulTransactionReturn> GetTransactionReceipt(SuccessfulTransactionReturn response);
    }
}