using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.Config;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Networking;
using HttpClient = Sequence.Utils.HttpClient;

namespace Sequence.Pay.Transak
{
    public class TransakOnRamp
    {
        private readonly Dictionary<string, string> _headers = new()
        {
            { "X-Access-Key", SequenceConfig.GetConfig().BuilderAPIKey }
        };
        
        private Address _walletAddress;

        public TransakOnRamp(Address walletAddress)
        {
            this._walletAddress = walletAddress;
        }

        public TransakOnRamp(string walletAddress)
        {
            this._walletAddress = new Address(walletAddress);
        }
        
        public static async Task<SupportedCountry[]> GetSupportedCountries()
        {
            using UnityWebRequest request = UnityWebRequest.Get("https://api.transak.com/api/v2/countries");
            string url = request.url;
            string curlRequest = $"curl -X GET {url}";
            try
            {
                await request.SendWebRequest();
                
                if (request.error != null || request.result != UnityWebRequest.Result.Success || request.responseCode < 200 || request.responseCode > 299)
                {
                    throw new Exception($"Error sending request to {url}: {request.responseCode} {request.error}");
                }
                else
                {
                    byte[] results = request.downloadHandler.data;
                    var responseJson = Encoding.UTF8.GetString(results);
                    try
                    {
                        SupportedCountriesResponse result = JsonConvert.DeserializeObject<SupportedCountriesResponse>(responseJson);
                        if (result == null)
                        {
                            throw new Exception("Unmarshalled response is null");
                        }

                        return result.response;
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Error unmarshalling response from {url}: {e.Message} | given: {responseJson}");
                    }
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception("HTTP Request failed: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.downloadHandler.data)  + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FormatException e)
            {
                throw new Exception("Invalid URL format: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.downloadHandler.data) + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FileLoadException e)
            {
                throw new Exception("File load exception: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.downloadHandler.data) + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (Exception e) {
                throw new Exception("An unexpected error occurred: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.downloadHandler.data) + "\nCurl-equivalent request: " + curlRequest);
            }
            finally
            {
                request.Dispose();
            }
        }
        
        public async Task<string> GetTransakLink(string fiatCurrency = "USD", string defaultFiatAmount = "50", string defaultCryptoCurrency = AddFundsSettings.DefaultCryptoCurrency, string networks = AddFundsSettings.DefaultNetworks, bool disableWalletAddressForm = true)
        {
            AddFundsSettings addFundsSettings = new AddFundsSettings(_walletAddress, fiatCurrency, defaultFiatAmount, defaultCryptoCurrency, networks);
            OnOffRampQueryParameters queryParameters = new OnOffRampQueryParameters(_walletAddress, addFundsSettings, disableWalletAddressForm);
            
            return await GetTransakWidgetUrl(queryParameters);
        }
        
        public async Task OpenTransakLink(string fiatCurrency = "USD", string defaultFiatAmount = "50", string defaultCryptoCurrency = AddFundsSettings.DefaultCryptoCurrency, string networks = AddFundsSettings.DefaultNetworks, bool disableWalletAddressForm = true)
        {
            var url = await GetTransakLink(fiatCurrency, defaultFiatAmount, defaultCryptoCurrency, networks,
                disableWalletAddressForm);
            
            Application.OpenURL(url);
        }

        private async Task<string> GetTransakWidgetUrl(OnOffRampQueryParameters @params)
        {
            const string path = "rpc/API/TransakGetWidgetURL";
            
#if SEQUENCE_DEV_STACK || SEQUENCE_DEV
            const string baseUrl = "https://dev-api.sequence.app";
#else
            const string baseUrl = "https://api.sequence.app";
#endif

            var args = new Dictionary<string, object>
            {
                { "params", @params }
            };

            var client = new HttpClient(baseUrl);
            
            var response = await client.SendPostRequest<
                Dictionary<string, object>, 
                Dictionary<string, string>
            >(path, args, _headers);
            
            return response.TryGetValue("url", out var url) ?  url : string.Empty;
        }
    }
}