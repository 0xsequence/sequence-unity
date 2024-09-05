using System.Threading.Tasks;
using Sequence.EmbeddedWallet;

namespace Sequence.Marketplace
{
    public class TransactionGenerator
    {
        private IWallet _wallet;
        private Chain _chain;
        private IHttpClient _client;

        public TransactionGenerator(IWallet wallet, Chain chain, IHttpClient client = null)
        {
            _wallet = wallet;
            _chain = chain;
            if (client == null)
            {
                client = new HttpClient();
            }
            _client = client;
        }

        public async Task<Step[]> GenerateBuyTransaction(Address collection, MarketplaceKind marketplace,
            OrderData[] ordersData, AdditionalFee[] additionalFees)
        {
            GenerateBuyTransaction generateBuyTransaction = new GenerateBuyTransaction(collection, _wallet.GetWalletAddress(),
                marketplace, ordersData, additionalFees, _wallet.GetWalletKind());

            Step[] steps =
                await _client.SendRequest<GenerateBuyTransaction, Step[]>(_chain, "GenerateBuyTransaction",
                    generateBuyTransaction);
            return steps;
        }
    }
}