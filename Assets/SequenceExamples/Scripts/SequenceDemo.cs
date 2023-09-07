using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sequence;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.Networking;
using System;
using System.Numerics;

public class SequenceDemo : MonoBehaviour
{

    //Fetching example, similar to web3-unity-sdk

    readonly string accountAddress = "0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";
     
    public async void LoadTokens()
    { 
        var tokenBalances = await Indexer.FetchMultiplePages(
            async (pageNumber) =>
            {
                GetTokenBalancesArgs tokenBalancesArgs = new GetTokenBalancesArgs(
                    accountAddress,
                    true,
                    new Page { page = pageNumber }
                );
                var balances = await Indexer.GetTokenBalances((int)Chain.Polygon, tokenBalancesArgs);

                return (balances.page, balances.balances);
            },
            9999
        );
        List<TokenBalance> tokenBalanceList = new List<TokenBalance>();
        foreach (var tokenBalance in tokenBalances)
        {
            var tokenBalanceWithContract = await Indexer.FetchMultiplePages(
                async (pageNumber) =>
                {
                    GetTokenBalancesArgs tokenBalancesArgs = new GetTokenBalancesArgs(
                        accountAddress,
                        tokenBalance.contractAddress,
                        true,
                        new Page { page = pageNumber }
                    );
                    var balances = await Indexer.GetTokenBalances(
                        (int)Chain.Polygon,
                        tokenBalancesArgs
                    );

                    return (balances.page, balances.balances);
                },
                9999
            );
            tokenBalanceList.AddRange(tokenBalanceWithContract);
        }

        await Task.WhenAll(tokenBalances.Select(async (tb) =>
        {
            //check for metadata
            var tokenMetadata = tb.tokenMetadata;
            var contractInfo = tb.contractInfo;
            var contractAddress = tb.contractAddress;
            Texture2D logoTex = null;

            var metaURL = tokenMetadata != null && tokenMetadata.image != null
                    && tokenMetadata.image.Length > 0
                    && !tokenMetadata.image.EndsWith("gif") ? tokenMetadata.image : ((contractInfo.logoURI != null && contractInfo.logoURI.Length > 0) ? contractInfo.logoURI : null);
            if (metaURL != null)
            {
                using (var imgRequest = UnityWebRequestTexture.GetTexture(metaURL))
                {
                    await imgRequest.SendWebRequest();

                    if (imgRequest.result != UnityWebRequest.Result.Success)
                    {

                        Debug.Log(metaURL + ", " + imgRequest.error);
                    }
                    else
                    {
                        // Create new card and initiate it
                        logoTex = ((DownloadHandlerTexture)imgRequest.downloadHandler).texture;
                    }
                }
            }

            var type = ContractType.UNKNOWN;
            try
            {
                type = Enum.Parse<ContractType>(contractInfo.type);
            }
            catch
            {
                // ok!
            }

            BigInteger? tokenID =
                (tokenMetadata != null)
                    ? tokenMetadata.tokenId
                    : null;

            switch (tb.contractType)
            {
                case ContractType.ERC20:
                    LoadCoins(tb);
                    break;
                case ContractType.ERC1155:
                case ContractType.ERC721:
                    LoadCollections(tb);
                    break;
                default:
                    break;
            }

        }));


    }

    //ERC20
    public async void LoadCoins(TokenBalance tokenBalance)
    {
        throw new NotImplementedException();
    }

    //ERC721 AND ERC1155
    public async void LoadCollections(TokenBalance tokenBalance)
    {
        throw new NotImplementedException();

    }

}
