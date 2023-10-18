using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.Demo
{
    public static class SpriteFetcher
    {
        private static string[] _validFileTypeIdentifiers = new string[]
        {
            ".jpg", ".jpeg", ".png", ".webp", ".bmp", ".tiff", ".tif", ".tgs", ".psd", ".hdr", ".svg", ".exr"
        };

        public static async Task<Sprite> Fetch(string url)
        {
            Texture2D texture = new Texture2D(100, 100); // Default if we fail to fetch the texture
            if (url != null && url.Length > 0 && !url.EndsWith(".gif"))
            {
                try
                {
                    UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(url);
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
                    Debug.LogError("HTTP Request failed: " + e.Message);
                } catch (FormatException e) {
                    Debug.LogError("Invalid URL format: " + e.Message);
                } catch (Exception e) {
                    Debug.LogError("An unexpected error occurred: " + e.Message);
                }
            }
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(.5f, .5f));
            return sprite;
        }
        
        private static bool EndsWithValidTypeIdentifier(this string url)
        {
            int identifiers = _validFileTypeIdentifiers.Length;
            for (int i = 0; i < identifiers; i++)
            {
                if (url.EndsWith(_validFileTypeIdentifiers[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}