using System;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sequence.Demo
{
    public class MockContentFetcher : INftContentFetcher
    {
        private int _totalFetchable = 30;
        private int _fetched = 0;
        public event Action<FetchNftContentResult> OnNftFetchSuccess;

        public async Task FetchContent(int maxToFetch)
        {
            int count = Math.Min(maxToFetch, _totalFetchable - _fetched);
            _fetched += count;
            Texture2D[] mockTextures = new Texture2D[count];
            for (int i = 0; i < count; i++)
            {
                mockTextures[i] = CreateMockTexture();
                await Task.Delay(100);
            }

            bool moreToFetch = _totalFetchable - _fetched > 0;

            OnNftFetchSuccess?.Invoke(new FetchNftContentResult(mockTextures, moreToFetch));
        }

        private Texture2D CreateMockTexture()
        {
            int width = 100;
            int height = 100;
            Texture2D mockTexture = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];
            int randomIndex = Random.Range(0, 5);
            for (int i = 0; i < pixels.Length; i++)
            {
                if (i % 5 == randomIndex)
                {
                    pixels[i] = Color.black;
                    randomIndex = Random.Range(0, 5);
                }
                else
                {
                    pixels[i] = Color.white;
                }
            }

            mockTexture.SetPixels(pixels);
            mockTexture.Apply();
            return mockTexture;
        }
    }
}