using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using Sequence;

public class GameStateLoader
{
    private Address _userAddress;
    private IIndexer _indexer;
    private LevelStarDictionary _levelStarDictionary;

    public GameStateLoader()
    {
        _userAddress = SequenceBridge.Wallet.GetWalletAddress();
        _indexer = SequenceBridge.Indexer;
        _levelStarDictionary = new LevelStarDictionary(_indexer, _userAddress);
    }

    public GameStateLoader(Address userAddress)
    {
        _userAddress = userAddress;
        _indexer = SequenceBridge.Indexer;
        _levelStarDictionary = new LevelStarDictionary(_indexer, _userAddress);
    }

    public async Task<int> GetStarsForLevel(string levelId, bool forceRefresh = false)
    {
        int stars = _levelStarDictionary.GetStarsForLevelId(levelId);
        if (forceRefresh || stars == -1)
        {
            await _levelStarDictionary.Build();
            stars = _levelStarDictionary.GetStarsForLevelId(levelId);
        }

        return Math.Max(0, stars);
    }

    public async Task<GetTokenBalancesReturn> GetTokens()
    {
        GetTokenBalancesReturn balances = await _indexer.GetTokenBalances(new GetTokenBalancesArgs(_userAddress, SequenceBridge.GameStateContractAddress));
        return balances;
    }
}
