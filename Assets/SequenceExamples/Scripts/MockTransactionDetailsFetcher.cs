using System;
using System.Threading.Tasks;
using NBitcoin.RPC;
using Sequence.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sequence.Demo
{
    public class MockTransactionDetailsFetcher : ITransactionDetailsFetcher
    {
        public event Action<FetchTransactionDetailsResult> OnTransactionDetailsFetchSuccess;

        private int _totalFetchable;
        private int _fetched = 0;
        public readonly int DelayInMilliseconds = 10;

        public MockTransactionDetailsFetcher(int totalFetchable = 5)
        {
            _totalFetchable = totalFetchable;
        }
        
        public async Task FetchTransactions(int maxToFetch)
        {
            int count = Math.Min(maxToFetch, _totalFetchable - _fetched);
            _fetched += count;
            TransactionDetails[] mockElements = new TransactionDetails[count];
            for (int i = 0; i < count; i++)
            {
                mockElements[i] = CreateMockElement();
                await Task.Delay(DelayInMilliseconds);
            }

            bool moreToFetch = _totalFetchable - _fetched > 0;
            
            OnTransactionDetailsFetchSuccess?.Invoke(new FetchTransactionDetailsResult(mockElements, moreToFetch));
        }

        public Task FetchTransactionsFromContract(string contractAddress, int maxToFetch)
        {
            return FetchTransactionsFromContracts(new string[] { contractAddress }, maxToFetch);
        }

        public async Task FetchTransactionsFromContracts(string[] contractAddresses, int maxToFetch)
        {
            if (contractAddresses == null || contractAddresses.Length == 0)
            {
                throw new ArgumentException($"{nameof(contractAddresses)} must not be null or empty");
            }
            
            int count = Math.Min(maxToFetch, _totalFetchable - _fetched);
            _fetched += count;
            TransactionDetails[] mockElements = new TransactionDetails[count];
            for (int i = 0; i < count; i++)
            {
                var temp = CreateMockElement();
                while (!temp.ContractAddress.Value.IsIn(contractAddresses))
                {
                    temp = CreateMockElement();
                }

                mockElements[i] = temp;
                await Task.Delay(DelayInMilliseconds);
            }

            bool moreToFetch = _totalFetchable - _fetched > 0;
            
            OnTransactionDetailsFetchSuccess?.Invoke(new FetchTransactionDetailsResult(mockElements, moreToFetch));
        }

        private TransactionDetails CreateMockElement()
        {
            string[] potentialTypes = new string[] { "Received", "Sent" };
            string[] potentialSymbols = new string[] { "ST", "MWS", "STT", "SST" };
            string[] potentialMockAddresses = new string[]
            {
                "0xc683a014955b75F5ECF991d4502427c8fa1Aa249", "0x1099542D7dFaF6757527146C0aB9E70A967f71C0",
                "0x606e6d28e9150D8A3C070AEfB751a2D0C5DB19fa", "0xb396CbD9b745Ffc4a9C9A6D43D7957b1350Be153"
            };
            Texture2D tokenIconTexture = MockNftContentFetcher.CreateMockTexture();
            Sprite tokenIconSprite = Sprite.Create(tokenIconTexture, new Rect(0, 0, tokenIconTexture.width, tokenIconTexture.height),
                new Vector2(.5f, .5f));
            
            int startYear = 2000;
            int endYear = 2023;
            int randomYear = Random.Range(startYear, endYear + 1);
            int randomMonth = Random.Range(1, 13);
            int daysInMonth = DateTime.DaysInMonth(randomYear, randomMonth);
            int randomDay = Random.Range(1, daysInMonth + 1);
            DateTime randomDate = new DateTime(randomYear, randomMonth, randomDay);
            string randomDateString = randomDate.ToString("MMMM d, yyyy");

            return new TransactionDetails(potentialTypes.GetRandomObjectFromArray(),
                EnumExtensions.GetRandomEnumValue<Chain>(),
                tokenIconSprite,
                new Address(potentialMockAddresses.GetRandomObjectFromArray()),
                new Address(potentialMockAddresses.GetRandomObjectFromArray()),
                new Address(potentialMockAddresses.GetRandomObjectFromArray()),
                (uint)Random.Range(0, 10000),
                potentialSymbols.GetRandomObjectFromArray(),
                randomDateString,
                new MockCurrencyConverter());
        }

        public void Refresh()
        {
            _fetched = 0;
        }
    }
}