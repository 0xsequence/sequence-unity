using UnityEngine;

namespace Sequence.EcosystemWallet.Browser
{
    public class WebBrowser : IBrowser
    {
        public void Show(string url)
        {
            Application.OpenURL(url);
        }
    }
}