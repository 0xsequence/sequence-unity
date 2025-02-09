using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Sequence.Demo.Utils;
using Sequence.EmbeddedWallet;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.Demo
{
    public static class AssetHandler
    {
        public static readonly Texture2D DefaultTexture = new Texture2D(100, 100); // Default if we fail to fetch the texture
        private static readonly string Directory = Path.Combine(Application.persistentDataPath, "assets");
        
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
            var cacheKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(url));
            if (url == null || url.Length <= 0 || url.EndsWith(".gif"))
                return texture;

            if (TryGetTexture(cacheKey, out var cachedTexture))
                return cachedTexture;
            
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
                    var data = texture.EncodeToPNG();
                    FileStorage.Save(data, cacheKey, Directory);
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
        
        public static bool TryGetTexture(string key, out Texture2D texture)
        {
            var data = FileStorage.Read(Path.Combine(Directory, key));
            if (data == null)
            {
                texture = null;
                return false;
            }

            texture = new Texture2D(1, 1);
            texture.LoadImage(data);
            return true;
        }
    }
}