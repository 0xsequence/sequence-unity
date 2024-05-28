using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Object = System.Object;

namespace Sequence.Authentication
{
    public class WebBrowser : IBrowser
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        public void SetState(string state)
        {

        }
        
        public void Authenticate(string url, string redirectUrl = "")
        {
            throw new System.Exception("Social sign in not supported in Web builds.");
        }
#else
        public void Authenticate(string url, string redirectUrl = "")
        {
            throw new NotImplementedException("Web browser is only supported on Web platforms.");
        }

        public void SetState(string state)
        {
            
        }
#endif
    }
}