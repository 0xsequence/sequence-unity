using System.Collections;
using System.Collections.Generic;
using Sequence;
using Sequence.Relayer;
using Sequence.WaaS;
using UnityEngine;

public static class SequenceBridge
{
    public static WaaSWallet Wallet;
    public const Chain Network = Chain.Polygon;
    public static IIndexer Indexer = new ChainIndexer(Network);
    public const string GameStateContractAddress = "0x64d9f9d527abe2a1c1ce3fada98601c4ac5bfdd2";
    public const string Level0TokenId = "123456"; // This is a placeholder token ID. For each level, the token ID is $"123456{n}" where n is the level number. The amount of each token ID you have is the number of stars you earned for that level.
    public static CloudflareMinter Minter;
    
    public static void OnTransactionFailedHandler(FailedTransactionReturn result)
    {
        Debug.LogError($"Failed to send transaction: {result.request} | reason: {result.error}");
    }
    
    public static void OnTransactionSuccessHandler(SuccessfulTransactionReturn result)
    {
        Debug.Log($"Successfully sent transaction: {result.txHash}");
        Application.OpenURL(ChainDictionaries.BlockExplorerOf[Network] + $"tx/{result.txHash}");
    }
    
    public static void OnMintSuccessHandler(string result)
    {
        Debug.Log($"Successfully minted token: {result}");
        Application.OpenURL(ChainDictionaries.BlockExplorerOf[Network] + $"tx/{result}");
    }

    public static void OnMintFailedHandler(string error)
    {
        Debug.LogError("Failed to mint token: " + error);
    }
}
