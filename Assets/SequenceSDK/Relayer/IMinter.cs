using System;
using System.Threading.Tasks;

namespace Sequence.Relayer
{
    public interface IMinter
    {
        public event Action<string> OnMintTokenSuccess;
        public event Action<string> OnMintTokenFailed;
        
        /// <summary>
        /// Mint amount of tokenId
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Task<string> MintToken(string tokenId, uint amount = 1);
    }
}