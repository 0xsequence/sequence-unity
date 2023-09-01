using System;
using System.Threading.Tasks;
using Sequence.Contracts;
using Sequence.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sequence.Demo
{
    public class MockTokenContentFetcher : ITokenContentFetcher
    {
        public event Action<FetchTokenContentResult> OnTokenFetchSuccess;

        private int _totalFetchable;
        private int _fetched = 0;
        public readonly int DelayInMilliseconds = 10;

        public MockTokenContentFetcher(int totalFetchable = 5)
        {
            _totalFetchable = totalFetchable;
        }
        
        public async Task FetchContent(int maxToFetch)
        {
            int count = Math.Min(maxToFetch, _totalFetchable - _fetched);
            _fetched += count;
            TokenElement[] mockElements = new TokenElement[count];
            for (int i = 0; i < count; i++)
            {
                mockElements[i] = CreateMockElement();
                await Task.Delay(DelayInMilliseconds);
            }

            bool moreToFetch = _totalFetchable - _fetched > 0;
            
            OnTokenFetchSuccess?.Invoke(new FetchTokenContentResult(mockElements, moreToFetch));
        }

        private TokenElement CreateMockElement()
        {
            string[] potentialNames = new string[]
                { "AwesomeToken", "MadeWithSequence", "SequenceSampleToken", "SequenceTestToken", "SequenceToken", "SequenceBucks" };
            string[] potentialSymbols = new string[] { "ST", "MWS", "STT", "SST" };
            string[] potentialMockAddresses = new string[]
            {
                "0xc683a014955b75F5ECF991d4502427c8fa1Aa249", "0x1099542D7dFaF6757527146C0aB9E70A967f71C0",
                "0x606e6d28e9150D8A3C070AEfB751a2D0C5DB19fa", "0xb396CbD9b745Ffc4a9C9A6D43D7957b1350Be153"
            };
            Texture2D tokenIconTexture = MockNftContentFetcher.CreateMockTexture();
            Sprite tokenIconSprite = Sprite.Create(tokenIconTexture, new Rect(0, 0, tokenIconTexture.width, tokenIconTexture.height),
                new Vector2(.5f, .5f));

            return new TokenElement(
                new ERC20(potentialMockAddresses.GetRandomObjectFromArray()),
                tokenIconSprite,
                potentialNames.GetRandomObjectFromArray(),
                EnumExtensions.GetRandomEnumValue<Chain>(),
                (uint)Random.Range(0, 10000),
                potentialSymbols.GetRandomObjectFromArray(),
                new MockCurrencyConverter());
        }

        public void Refresh()
        {
            _fetched = 0;
        }
    }
}