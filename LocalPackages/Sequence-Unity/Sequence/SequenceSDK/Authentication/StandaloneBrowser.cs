using UnityEngine;

namespace Sequence.Authentication
{
    public class StandaloneBrowser : IBrowser
    {
        public void Authenticate(string url, string redirectUrl = "")
        {
            Application.OpenURL(url);
        }

        public void SetState(string state)
        {
            
        }
    }
}