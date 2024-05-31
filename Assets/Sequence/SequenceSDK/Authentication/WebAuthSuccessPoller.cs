using System;
using System.Collections;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.Authentication
{
    public class WebAuthSuccessPoller : MonoBehaviour
    {
        private OpenIdAuthenticator _authenticator;
        private string _authenticationUrl;
        private string _state;
        private WaitForSecondsRealtime _delayBetweenChecks = new WaitForSecondsRealtime(.1f);

        public void Setup(OpenIdAuthenticator authenticator, string authenticationUrl, string state)
        {
            _authenticator = authenticator;
            _authenticationUrl = authenticationUrl;
            _state = state;
        }

        public void PollForAuthSuccess()
        {
            StartCoroutine(PollForSuccess());
        }

        private IEnumerator PollForSuccess()
        {
            while (true)
            {
                yield return _delayBetweenChecks;

                Task<bool> request = SendGetSuccessRequest();
                yield return new WaitUntil(() => request.IsCompleted);

                if (request.Result)
                {
                    break;
                }
            }
            
            Destroy(gameObject);
        }

        private async Task<bool> SendGetSuccessRequest()
        {
            string url = _authenticationUrl.RemoveTrailingSlash() + $"/getResult?state={_state}";
            UnityWebRequest request = UnityWebRequest.Get(url);
            string curlRequest = $"curl {url}";

            try
            {
                await request.SendWebRequest();
                if (request.error != null || request.result != UnityWebRequest.Result.Success || request.responseCode < 200 || request.responseCode > 299)
                {
                    throw new HttpRequestException($"Error sending request to {url}: {request.responseCode} {request.error}");
                }
                else
                {
                    string response = request.downloadHandler.text;
                    request.Dispose();
                    if (!string.IsNullOrWhiteSpace(response))
                    {
                        _authenticator.HandleDeepLink(response);
                        return true;
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Debug.LogError("HTTP Request failed: " + e.Message);
            }
            catch (FormatException e)
            {
                Debug.LogError("Invalid URL format: " + e.Message + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FileLoadException e)
            {
                Debug.LogError("File load exception: " + e.Message + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (Exception e) {
                Debug.LogError("An unexpected error occurred: " + e.Message + "\nCurl-equivalent request: " + curlRequest);
            }

            return false;
        }
    }
}