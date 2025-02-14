using System;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence
{
    public class DownloadHandlerStream<T> : DownloadHandlerScript
    {
        private readonly WebRPCStreamOptions<T> _options;
        
        public DownloadHandlerStream(WebRPCStreamOptions<T> options)
        {
            _options = options;
        }
        
        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            CompleteContent();
            
            // Stream returns a ping of size 1
            if (dataLength <= 1)
                return true;

            try
            {
                // One event can include multiple JSON objects, seperated by a line break
                var input = Encoding.UTF8.GetString(data);
                var jsonParts = input.Split(Environment.NewLine.ToCharArray());

                foreach (var part in jsonParts)
                {
                    if (!part.StartsWith('{'))
                        continue;
                    
                    var streamData = JsonConvert.DeserializeObject<T>(part);
                    _options.onMessage?.Invoke(streamData);   
                }
                
                return true;
            }
            catch (Exception e)
            {
                _options.onError?.Invoke(new WebRPCError
                {
                    msg = e.Message
                });
                
                Debug.LogException(e);
                return false;
            }
        }
    }
}