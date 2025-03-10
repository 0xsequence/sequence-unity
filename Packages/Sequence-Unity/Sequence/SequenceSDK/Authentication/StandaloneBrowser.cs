namespace Sequence.Authentication
{
    public class StandaloneBrowser : IBrowser
    {
        public void Authenticate(string url, string redirectUrl = "")
        {
#if UNITY_2017_1_OR_NEWER
            UnityEngine.Application.OpenURL(url);
#endif
        }

        public void SetState(string state)
        {
            
        }
    }
}