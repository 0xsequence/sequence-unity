using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.EmbeddedWallet;

namespace Sequence.Integrations.Transak
{
    public class TransakOnRamp
    {
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
            var url = "https://api.transak.com/api/v2/countries";
            using IWebRequest request = WebRequestBuilder.Get(url);
            string curlRequest = $"curl -X GET {url}";
            try
            {
                var response = await request.Send();
                
                if (request.Error != null || response.Result != WebRequestResult.Success || response.ResponseCode < 200 || response.ResponseCode > 299)
                {
                    throw new Exception($"Error sending request to {url}: {response.ResponseCode} {request.Error}");
                }
                else
                {
                    byte[] results = request.Data;
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
                throw new Exception("HTTP Request failed: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.Data)  + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FormatException e)
            {
                throw new Exception("Invalid URL format: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.Data) + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FileLoadException e)
            {
                throw new Exception("File load exception: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.Data) + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (Exception e) {
                throw new Exception("An unexpected error occurred: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.Data) + "\nCurl-equivalent request: " + curlRequest);
            }
            finally
            {
                request.Dispose();
            }
        }
        
        public string GetTransakLink(string fiatCurrency = "USD", string defaultFiatAmount = "50", string defaultCryptoCurrency = AddFundsSettings.DefaultCryptoCurrency, string networks = AddFundsSettings.DefaultNetworks, bool disableWalletAddressForm = true)
        {
            AddFundsSettings addFundsSettings = new AddFundsSettings(_walletAddress, fiatCurrency, defaultFiatAmount, defaultCryptoCurrency, networks);
            OnOffRampQueryParameters queryParameters = new OnOffRampQueryParameters(_walletAddress, addFundsSettings, disableWalletAddressForm);
            return $"https://global.transak.com?{queryParameters.AsQueryParameters()}";
        }
        
        public void OpenTransakLink(string fiatCurrency = "USD", string defaultFiatAmount = "50", string defaultCryptoCurrency = AddFundsSettings.DefaultCryptoCurrency, string networks = AddFundsSettings.DefaultNetworks, bool disableWalletAddressForm = true)
        {
#if UNITY_2017_1_OR_NEWER
            UnityEngine.Application.OpenURL(GetTransakLink(fiatCurrency, defaultFiatAmount, defaultCryptoCurrency, networks, disableWalletAddressForm));
#endif
        }
    }
}