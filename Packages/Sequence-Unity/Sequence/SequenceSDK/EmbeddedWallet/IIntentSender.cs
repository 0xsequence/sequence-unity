using System.Threading.Tasks;

namespace Sequence.EmbeddedWallet
{
    public interface IIntentSender
    {
        public Task<T> SendIntent<T, T2>(T2 args, IntentType type, uint timeBeforeExpiryInSeconds = 30, uint currentTime = 0);
        public Task<bool> DropSession(string dropSessionId);
        public Task<T> PostIntent<T>(string payload, string path);
        public Task<Session[]> ListSessions();
        public Task<SuccessfulTransactionReturn> GetTransactionReceipt(SuccessfulTransactionReturn response);
    }
}