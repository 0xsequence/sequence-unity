using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Browser
{
    public class TcpRedirectHandler : RedirectHandler
    {
        public static readonly int WindowsIpcPort = 52836;

        private static bool _running;
        private static string _response;
        
        public override async Task<(bool Result, TResponse Data)> WaitForResponse<TPayload, TResponse>(string url, string action, TPayload payload)
        {
            _response = string.Empty;
            
            PrepareServer("_redirectUrl");
            StartServer();

            var finalUrl = ConstructUrl(url, action, payload);
            Application.OpenURL(finalUrl);
            
            while (string.IsNullOrEmpty(_response))
                await Task.Delay(100);
            
            Debug.Log($"Received Response from tcp: {_response}");
            
            var data = _response.ExtractQueryAndHashParameters();
            
            Debug.Log($"Query Data: {JsonConvert.SerializeObject(data)}");
            
            var id = data["id"];
            if (id != Id)
                throw new Exception("Invalid request id");

            if (data.TryGetValue("error", out var error))
                throw new Exception(error);
            
            var responsePayloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(Uri.UnescapeDataString(data["payload"])));
            var responsePayload = JsonConvert.DeserializeObject<TResponse>(responsePayloadJson);

            return (true, responsePayload);
        }
        
        /// <summary>
        /// Run a TCP server on Windows standalone to get the auth token from the other instance.
        /// IMPORTANT NOTE: Do not add code to this TCP server/client without thinking very carefully; it's easy to get driveby exploited since this is a TCP server any application can talk to.
        /// IMPORTANT NOTE: Do not increase the attack surface without careful thought.
        /// </summary>
        private static void StartServer() 
        {
            var syncContext = SynchronizationContext.Current;
            var ipcListener = new Thread(() =>
            {
                var socketConnection = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), WindowsIpcPort);
                socketConnection.Start();
                var bytes = new System.Byte[1024];
                var msg = "";
                
                while (true)
                {
                    using (var connectedTcpClient = socketConnection.AcceptTcpClient())
                    {
                        using (NetworkStream stream = connectedTcpClient.GetStream())
                        {
                            int length;
                            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                var data = new byte[length];
                                System.Array.Copy(bytes, 0, data, 0, length);
                                // Convert byte array to string message. 							
                                string clientMessage = System.Text.Encoding.ASCII.GetString(data);
                                if (clientMessage.StartsWith("@@@@"))
                                {
                                    msg = clientMessage.Replace("@@@@", "").Replace("$$$$", "");
                                }
                                else
                                {
                                    msg += clientMessage.Replace("$$$$", "");
                                }
                                if (msg.Length > 8192)
                                {
                                    // got some weird garbage, toss it to avoid memory leaks.
                                    msg = "";
                                }
                                if (clientMessage.EndsWith("$$$$"))
                                {
                                    syncContext.Post((data) =>
                                    {
                                        Debug.Log($"{(string)data}");
                                        _response = (string)data;
                                    }, msg);
                                }
                            }
                        }
                    }
                }
            });
            
            ipcListener.IsBackground = true;
            ipcListener.Start();
            _running = true;
        }

        private static void PrepareServer(string redirectUrl)
        {
            if (_running)
                return;

            // Register a Windows URL protocol handler in the Windows Registry.
            var scheme = redirectUrl.Replace("://", string.Empty);
            var appPath = Path.GetFullPath(Application.dataPath.Replace("_Data", ".exe"));
            
            string[] commands = new string[]{
                $"add HKEY_CURRENT_USER\\Software\\Classes\\{scheme} /t REG_SZ /d \"URL:Sequence Login for {Application.productName}\" /f",
                $"add HKEY_CURRENT_USER\\Software\\Classes\\{scheme} /v \"URL Protocol\" /t REG_SZ /d \"\" /f",
                $"add HKEY_CURRENT_USER\\Software\\Classes\\{scheme}\\shell /f",
                $"add HKEY_CURRENT_USER\\Software\\Classes\\{scheme}\\shell\\open /f",
                $"add HKEY_CURRENT_USER\\Software\\Classes\\{scheme}\\shell\\open\\command /t REG_SZ /d \"\\\"{appPath}\\\" \\\"%1\\\"\" /f",
            };
            
#if ENABLE_MONO
            foreach(var args in commands) {
                var command = new System.Diagnostics.ProcessStartInfo();
                command.FileName = "C:\\Windows\\System32\\reg.exe";
                command.Arguments = args;
                command.UseShellExecute = false;
                command.CreateNoWindow = true;
                System.Diagnostics.Process.Start(command);
            }
#elif ENABLE_IL2CPP
            try
            {
                foreach(var args in commands) {
                    var command = new System.Diagnostics.ProcessStartInfo();
                    command.FileName = "C:\\Windows\\System32\\reg.exe";
                    command.Arguments = args;
                    command.UseShellExecute = false;
                    command.CreateNoWindow = true;
                    System.Diagnostics.Process.Start(command);
                }
            }
            catch (Exception ex)
            {
                string message = $"Failed to register URL scheme '{scheme}': {ex.Message}" + "\nSocial sign in is not currently supported on IL2CPP";
                Debug.LogWarning(message);
                throw new Exception(message);
            }
#endif
            
        }
    }
}