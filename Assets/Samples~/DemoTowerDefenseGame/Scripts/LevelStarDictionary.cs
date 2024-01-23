using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence;

namespace DefaultNamespace
{
    public class LevelStarDictionary
    {
        private Dictionary<string, int> _levelStars = new Dictionary<string, int>();
        private IIndexer _indexer;
        private Address _userAddress;

        private bool _isBuilding = false;

        public LevelStarDictionary(IIndexer indexer, Address userAddress)
        {
            _indexer = indexer;
            _userAddress = userAddress;
        }

        public async Task Build()
        {
            while (_isBuilding)
            {
                await Task.Yield();
                return;
            }
            
            _isBuilding = true;
            GetTokenBalancesReturn balances = await _indexer.GetTokenBalances(new GetTokenBalancesArgs(_userAddress, SequenceBridge.GameStateContractAddress));
            for (int i = 0; i < balances.balances.Length; i++)
            {
                string id = balances.balances[i].tokenID.ToString();
                if (id.StartsWith(SequenceBridge.Level0TokenId))
                {
                    _levelStars[id] = (int)balances.balances[i].balance;
                }
            }
            _isBuilding = false;
        }
        
        public int GetStarsForLevelId(string levelId)
        {
            string id = SequenceBridge.Level0TokenId + levelId;
            int toReturn = 0;
            if (_levelStars.ContainsKey(id))
            {
                toReturn = _levelStars[id];
                _levelStars.Remove(id);
            }
            else if (_levelStars.Count == 0)
            {
                toReturn = -1;
            }

            return toReturn;
        }
    }
}