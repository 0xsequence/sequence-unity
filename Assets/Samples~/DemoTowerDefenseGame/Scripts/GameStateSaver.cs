using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence;
using Sequence.Contracts;
using Sequence.WaaS;
using SequenceSDK.WaaS;
using UnityEngine;
using Transaction = SequenceSDK.WaaS.Transaction;

public class GameStateSaver
{
    private IWallet _wallet;
    private GameStateLoader _loader;
    private Address _walletAddress;
    private ERC1155 _gameStateContract;
    
    public GameStateSaver(IWallet wallet = null)
    {
        _wallet = wallet;
        if (_wallet == null)
        {
            _wallet = SequenceBridge.Wallet;
        }

        _walletAddress = _wallet.GetWalletAddress();
        _loader = new GameStateLoader(_walletAddress);
        _gameStateContract = new ERC1155(SequenceBridge.GameStateContractAddress);
    }

    public async Task CompleteLevel(string levelId, int starsEarned)
    {
        int stars = await _loader.GetStarsForLevel(levelId, forceRefresh: true);
        if (starsEarned > stars)
        {
            int needed = starsEarned - stars;
            _wallet.SendTransaction(SequenceBridge.Network, new Transaction[]
            {
                new RawTransaction(_gameStateContract.Mint(_walletAddress, BigInteger.Parse(SequenceBridge.Level0TokenId + levelId), needed)),
            });
        }
    }

    public async Task ResetSave()
    {
        GetTokenBalancesReturn balances = await _loader.GetTokens();
        Transaction[] transactions = new Transaction[balances.balances.Length];
        for (int i = 0; i < balances.balances.Length; i++)
        {
            transactions[i] = new RawTransaction(_gameStateContract.Burn(balances.balances[i].tokenID, balances.balances[i].balance));
        }

        if (transactions.Length > 0)
        {
            await _wallet.SendTransaction(SequenceBridge.Network, transactions);
        }
    }
}