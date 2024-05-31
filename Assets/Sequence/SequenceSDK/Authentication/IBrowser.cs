using System.Threading.Tasks;

namespace Sequence.Authentication
{
    public interface IBrowser
    {
        /// <summary>
        /// Authenticate the user with the social sign in provider url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="redirectUrl"></param>
        public void Authenticate(string url, string redirectUrl = "");
        
        /// <summary>
        /// For some implementations of IBrowser, setting/re-setting the state token may be required
        /// </summary>
        /// <param name="state"></param>
        public void SetState(string state);
    }
}