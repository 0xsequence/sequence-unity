using System.Threading.Tasks;

namespace Sequence.Authentication
{
    public interface IBrowser
    {
        public void Authenticate(string url, string redirectUrl = "");
    }
}