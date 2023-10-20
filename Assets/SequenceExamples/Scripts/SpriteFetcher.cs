using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.Demo
{
    public static class SpriteFetcher
    {
        public static async Task<Sprite> Fetch(string url)
        {
            Texture2D texture = new Texture2D(100, 100); // Default if we fail to fetch the texture
            if (url != null && url.Length > 0 && !url.EndsWith(".gif"))
            {
                try
                {
                    using UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(url);
                    await imageRequest.SendWebRequest();

                    if (imageRequest.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogWarning($"Error fetching image at url: {url}\nError: {imageRequest.error}\nDownload Handler error: {imageRequest.downloadHandler.error}\nReturning default");
                    }
                    else
                    {
                        texture = ((DownloadHandlerTexture) imageRequest.downloadHandler).texture;
                    }
                } catch (HttpRequestException e) {
                    Debug.LogWarning("HTTP Request failed: " + e.Message);
                } catch (FormatException e) {
                    Debug.LogWarning("Invalid URL format: " + e.Message);
                } catch (Exception e) {
                    if (e.Message.Contains($"{(int)HttpStatusCode.Gone}"))
                    {
                        Debug.LogWarning($"Error fetching image at url: {url}\nError: {e.Message}\nReturning default");
                    }
                    else
                    {
                        Debug.LogWarning("An unexpected error occurred: " + e.Message + $"\nUrl: {url}\nReturning default");
                    }
                }
            }
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(.5f, .5f));
            return sprite;
        }
    }
}