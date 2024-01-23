using System.Threading.Tasks;

namespace Sequence.Authentication
{
    public interface IEmailSignIn
    {
        public Task<string> SignIn(string email);
        public Task<string> Login(string challengeSession, string email, string code);
        public Task SignUp(string email);
    }
}