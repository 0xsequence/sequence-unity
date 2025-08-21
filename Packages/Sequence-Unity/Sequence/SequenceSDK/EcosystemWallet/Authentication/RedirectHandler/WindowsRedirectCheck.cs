#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
using System;
using System.Net.Sockets;
using System.Threading;
#endif

using UnityEngine;

namespace Sequence.EcosystemWallet.Browser
{
    public class WindowsRedirectCheck : MonoBehaviour
    {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        private static Mutex _mutex;
        
        private void Start()
        {
            return;
            if (IsFirstInstance())
                return;
            
            TrySendArgsToRunningInstance(Environment.GetCommandLineArgs());
            Application.Quit();
        }
        
        private void TrySendArgsToRunningInstance(string[] args)
        {
            if (args.Length <= 1) 
                return;

            try
            {
                using var client = new TcpClient("127.0.0.1", TcpRedirectHandler.WindowsIpcPort);
                using var stream = client.GetStream();
                
                var message = "@@@@" + args[1] + "$$$$";
                var bytes = System.Text.Encoding.ASCII.GetBytes(message);
                stream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to send deep link to running instance: {e.Message}");
            }
        }

        private bool IsFirstInstance()
        {
            _mutex = new Mutex(true, "SequenceWindowsInstance", out var createdNew);
            return createdNew;
        }
#endif
    }
}