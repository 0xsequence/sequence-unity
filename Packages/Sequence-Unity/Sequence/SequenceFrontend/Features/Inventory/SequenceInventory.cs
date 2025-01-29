using System.Linq;
using Sequence;
using Sequence.Demo;
using Sequence.EmbeddedWallet;
using UnityEngine;

namespace SequenceSDK.Samples
{
    public class SequenceInventory : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private Chain _chain = Chain.TestnetArbitrumSepolia;
        [SerializeField] private string _contractAddress = "0x00";
        
        [Header("Components")]
        [SerializeField] private GenericObjectPool<SequenceInventoryTile> _tilePool;

        private IWallet _wallet;
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void Show(IWallet wallet)
        {
            _wallet = wallet;
            gameObject.SetActive(true);
            LoadBalances();
        }

        private async void LoadBalances()
        {
            var indexer = new ChainIndexer(_chain);
            var args = new GetTokenBalancesArgs(_wallet.GetWalletAddress(), _contractAddress, true);
            var balances = await indexer.GetTokenBalances(args);
            var tokens = balances.balances.Select(b => b.tokenMetadata).ToArray();
            LoadTiles(tokens);
        }

        private void LoadTiles(TokenMetadata[] tokens)
        {
            _tilePool.Cleanup();
            foreach (var token in tokens)
                _tilePool.GetObject().Load(token, null);
        }
    }
}
