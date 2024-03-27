namespace Sequence.Authentication
{
    public class WebGLBrowser : IBrowser
    {
        public void Authenticate(string url, string redirectUrl = "")
        {
            throw new System.Exception("Social sign in not supported in WebGL.");
        }
    }
}