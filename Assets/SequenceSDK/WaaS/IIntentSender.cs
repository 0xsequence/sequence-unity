using System.Threading.Tasks;
using Sequence.WaaS.Authentication;

namespace Sequence.WaaS
{
    public interface IIntentSender
    {
        public Task<T> SendIntent<T, T2>(T2 args);
        public Task<bool> DropSession(string dropSessionId);
        public Task<T> PostIntent<T>(string payload, string path);
        public Task<WaaSSession[]> ListSessions();
    }
}