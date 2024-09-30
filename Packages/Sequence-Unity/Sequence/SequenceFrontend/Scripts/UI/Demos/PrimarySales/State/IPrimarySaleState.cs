using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.EmbeddedWallet;

namespace Sequence.Demo
{
    public interface IPrimarySaleState
    {
        string PaymentTokenSymbol { get; }
        BigInteger PaymentTokenDecimals { get; }
        BigInteger UserPaymentBalance { get; }
        BigInteger Cost { get; }
        BigInteger SupplyCap { get; }
        int StartTime { get; }
        int EndTime { get; }
        int TotalMinted { get; }
        Dictionary<BigInteger, TokenSupply> TokenSupplies { get; }
        Task Construct(string saleContractAddress, string tokenContractAddress, 
            IWallet wallet, Chain chain, int[] itemsForSale);
        Task<bool> Purchase(BigInteger tokenId, int amount);
    }
}