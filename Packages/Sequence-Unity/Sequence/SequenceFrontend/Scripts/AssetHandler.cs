using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.Demo
{
    public static class AssetHandler
    {
        public static readonly Texture2D DefaultTexture = new Texture2D(100, 100); // Default if we fail to fetch the texture
        
        public static async Task<Sprite> GetSpriteAsync(string url)
        {
            var texture = await GetTexture2DAsync(url);
            var sprite = Sprite.Create(texture, 
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(.5f, .5f));
            
            return sprite;
        }
        
        public static async Task<Texture2D> GetTexture2DAsync(string url)
        {
            var texture = DefaultTexture;
            if (url == null || url.Length <= 0 || url.EndsWith(".gif"))
                return texture;
            
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

            return texture;
        }
    }
}