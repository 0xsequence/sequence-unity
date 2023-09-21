using System;
using System.Threading.Tasks;
using Sequence.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sequence.Demo
{
    public class MockNftContentFetcher : INftContentFetcher
    {
        private int _totalFetchable;
        private int _fetched = 0;
        public event Action<FetchNftContentResult> OnNftFetchSuccess;
        public readonly int DelayInMilliseconds = 100;

        public MockNftContentFetcher(int totalFetchable = 30, int delayInMilliseconds = 100)
        {
            this._totalFetchable = totalFetchable;
            this.DelayInMilliseconds = delayInMilliseconds;
        }

        public async Task FetchContent(int maxToFetch)
        {
            int count = Math.Min(maxToFetch, _totalFetchable - _fetched);
            _fetched += count;
            NftElement[] mockNfts = new NftElement[count];
            for (int i = 0; i < count; i++)
            {
                mockNfts[i] = CreateMockContent();
                await Task.Delay(DelayInMilliseconds);
            }

            bool moreToFetch = _totalFetchable - _fetched > 0;

            OnNftFetchSuccess?.Invoke(new FetchNftContentResult(mockNfts, moreToFetch));
        }

        private NftElement CreateMockContent()
        {
            string[] potentialNames = new string[]
                { "AwesomeToken", "MadeWithSequence", "SequenceSampleToken", "SequenceTestToken", "SequenceToken", "SequenceBucks" };
            string[] potentialMockAddresses = new string[]
            {
                "0xc683a014955b75F5ECF991d4502427c8fa1Aa249", "0x1099542D7dFaF6757527146C0aB9E70A967f71C0",
                "0x606e6d28e9150D8A3C070AEfB751a2D0C5DB19fa", "0xb396CbD9b745Ffc4a9C9A6D43D7957b1350Be153"
            };
            Texture2D tokenIconTexture = CreateMockTexture();
            Sprite tokenIconSprite = Sprite.Create(tokenIconTexture, new Rect(0, 0, tokenIconTexture.width, tokenIconTexture.height),
                new Vector2(.5f, .5f));
            Texture2D collectionIconTexture = CreateMockTexture();
            Sprite collectionIconSprite = Sprite.Create(collectionIconTexture, new Rect(0, 0, collectionIconTexture.width, collectionIconTexture.height),
                new Vector2(.5f, .5f));
            
            return new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), 
                tokenIconSprite,
                potentialNames.GetRandomObjectFromArray(),
                collectionIconSprite,
                potentialNames.GetRandomObjectFromArray(),
                (uint)Random.Range(0, 10000),
                Chain.Ethereum,
                (uint)Random.Range(1, 30),
                Random.Range(0, 10000),
                new MockCurrencyConverter());
        }

        public static Texture2D CreateMockTexture()
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

        public void Refresh()
        {
            _fetched = 0;
        }
    }
}