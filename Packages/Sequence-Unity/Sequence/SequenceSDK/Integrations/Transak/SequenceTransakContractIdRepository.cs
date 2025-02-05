using System.Collections.Generic;
using Sequence.Marketplace;

namespace Sequence.Integrations.Transak
{
    public class SequenceTransakContractIdRepository
    {
        public const string ApiKey = "5911d9ec-46b5-48fa-a755-d59a715ff0cf"; // This can be hardcoded as it is a public API key

        public static Dictionary<Chain, Dictionary<OrderbookKind, TransakContractId>> SequenceContractIds =
            new Dictionary<Chain, Dictionary<OrderbookKind, TransakContractId>>()
            {
                {
                    Chain.Polygon, new Dictionary<OrderbookKind, TransakContractId>()
                    {
                        {
                            OrderbookKind.reservoir,
                            new TransakContractId("6675a6d0f597abb8f3e2e9c2",
                                new Address("0xc2c862322e9c97d6244a3506655da95f05246fd8"), Chain.Polygon, "MATIC")
                        },
                    }
                },
                {
                    Chain.ArbitrumOne, new Dictionary<OrderbookKind, TransakContractId>()
                    {
                        {
                            OrderbookKind.sequence_marketplace_v2,
                            new TransakContractId("66c5a2cf2fb1688e11fcb167", 
                                new Address("0xB537a160472183f2150d42EB1c3DD6684A55f74c"), Chain.ArbitrumOne, "USDC")
                        },
                        {
                            OrderbookKind.sequence_marketplace_v1,
                            new TransakContractId("66c5a2d8c00223b9cc6edfdc", new Address("0xfdb42A198a932C8D3B506Ffa5e855bC4b348a712"), Chain.ArbitrumOne, "USDC")
                        }
                    }
                }
            };
    }
}