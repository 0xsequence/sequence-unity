using UnityEditor;
using UnityEngine;
using System.Text;
using Sequence;
using Sequence.Sidekick;
using Sequence.Provider;
using Sequence.SidekickEditor;

public class SidekickEditorController : MonoBehaviour
{

    private SequenceSidekickClient sidekick = new SequenceSidekickClient(Chain.TestnetArbitrumSepolia);

    #region Sidekick Wallet

    public void GetWalletAddress()
    {
        EditorApplication.delayCall += async () =>
        {
            try
            {
                string result = await sidekick.GetWalletAddress();
                Debug.Log(result);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to recover sidekick wallet address: " + ex.Message);
            }
        };
    }

    #endregion

    #region Sidekick Contracts

    public void GetContracts()
    {
        EditorApplication.delayCall += async () =>
        {
            try
            {
                string result = await sidekick.GetAllContracts();
                Debug.Log(result);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to recover builder contracts: " + ex.Message);
            }
        };
    }

    #endregion

    #region Deploy

    #region Deploy ERC20

    public string DeployERC20InitialOwner { get; set; }
    public string DeployERC20Name { get; set; }
    public string DeployERC20Symbol { get; set; }

    public async void DeployERC20()
    {

        var payload = new DeployERC20Payload
        {
            initialOwner = DeployERC20InitialOwner,
            name = DeployERC20Name,
            symbol = DeployERC20Symbol
        };

        string jsonString = JsonUtility.ToJson(payload);

        Debug.Log("Deploying ERC20: " + jsonString);

        try
        {
            string result = await sidekick.DeployERC20(jsonString);

            DeployResult deployResult = JsonUtility.FromJson<DeployResult>(result);

            if (!string.IsNullOrEmpty(deployResult.result.txHash))
            {

                var receipt = await new SequenceEthClient(sidekick.Chain).WaitForTransactionReceipt(deployResult.result.txHash);

                if (receipt != null)
                {
                    string deployedAddress = ReceiptExtractor.ExtractFirstContractAddressExceptOwn(receipt, DeployERC20InitialOwner);
                    if (!string.IsNullOrEmpty(deployedAddress))
                    {
                        Debug.Log($"Deployed contract address: {deployedAddress}");
                    }
                    else
                    {
                        Debug.LogWarning("Could not extract deployed contract address from receipt.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to get transaction receipt.");
                }
            }
            else
            {
                Debug.LogError($"Deployment error: {deployResult.result.error}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Deploy ERC20 failed: " + ex.Message);
        }
    }

    #endregion

    #region Deploy ERC721
    public string DeployERC721DefaultAdmin { get; set; }
    public string DeployERC721Minter { get; set; }
    public string DeployERC721Name { get; set; }
    public string DeployERC721Symbol { get; set; }

    public async void DeployERC721()
    {
        var payload = new DeployERC721Payload
        {
            defaultAdmin = DeployERC721DefaultAdmin,
            minter = DeployERC721Minter,
            name = DeployERC721Name,
            symbol = DeployERC721Symbol
        };

        string jsonString = JsonUtility.ToJson(payload);

        Debug.Log("Deploying ERC721: " + jsonString);

        try
        {
            string result = await sidekick.DeployERC721(jsonString);

            DeployResult deployResult = JsonUtility.FromJson<DeployResult>(result);

            if (!string.IsNullOrEmpty(deployResult.result.txHash))
            {

                var receipt = await new SequenceEthClient(sidekick.Chain).WaitForTransactionReceipt(deployResult.result.txHash);

                if (receipt != null)
                {
                    string deployedAddress = ReceiptExtractor.ExtractFirstContractAddressExceptOwn(receipt, DeployERC721DefaultAdmin);
                    if (!string.IsNullOrEmpty(deployedAddress))
                    {
                        Debug.Log($"Deployed contract address: {deployedAddress}");
                    }
                    else
                    {
                        Debug.LogWarning("Could not extract deployed contract address from receipt.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to get transaction receipt.");
                }
            }
            else
            {
                Debug.LogError($"Deployment error: {deployResult.result.error}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Deploy ERC721 failed: " + ex.Message);
        }
    }

    #endregion

    #region Deploy ERC1155

    public string DeployDefaultAdmin { get; set; }
    public string DeployMinter { get; set; }
    public string DeployName { get; set; }

    public async void DeployERC1155()
    {
        var deployPayload = new DeployERC1155Payload
        {
            defaultAdmin = DeployDefaultAdmin,
            minter = DeployMinter,
            name = DeployName
        };

        string jsonString = JsonUtility.ToJson(deployPayload);

        Debug.Log("Deploying ERC1155: " + jsonString);

        try
        {
            string result = await sidekick.DeployERC1155(jsonString);

            DeployResult deployResult = JsonUtility.FromJson<DeployResult>(result);

            if (!string.IsNullOrEmpty(deployResult.result.txHash))
            {
                var receipt = await new SequenceEthClient(sidekick.Chain).WaitForTransactionReceipt(deployResult.result.txHash);

                if (receipt != null)
                {
                    string deployedAddress = ReceiptExtractor.ExtractFirstContractAddressExceptOwn(receipt, DeployDefaultAdmin);
                    if (!string.IsNullOrEmpty(deployedAddress))
                    {
                        Debug.Log($"Deployed contract address: {deployedAddress}");
                    }
                    else
                    {
                        Debug.LogWarning("Could not extract deployed contract address from receipt.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to get transaction receipt.");
                }
            }
            else
            {
                Debug.LogError($"Deployment error: {deployResult.result.error}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Deploy ERC1155 failed: " + ex.Message);
        }
    }

    #endregion

    #endregion

    #region Role

    public ContractRole SelectedContractRole { get; set; }
    public string GrantRoleContractAddress { get; set; }
    public string GrantRoleRole { get; set; }
    public string GrantRoleAccount { get; set; }

    public void GrantERC1155Role()
    {
        var grantRolePayload = new GrantRolePayload
        {
            role = GrantRoleRole,
            account = GrantRoleAccount
        };

        string jsonString = JsonUtility.ToJson(grantRolePayload);

        Debug.Log("Granting role ERC1155: " + jsonString);

        EditorApplication.delayCall += async () =>
        {
            try
            {
                string result = await sidekick.GrantRoleERC1155(GrantRoleContractAddress, jsonString);
                Debug.Log("Grant role result: " + result);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Grant ERC1155 role failed: " + ex.Message);
            }
        };
    }

    #endregion

    #region Mint

    #region MintERC20

    public string MintERC20ContractAddress { get; set; }
    public string MintERC20RecipientAddress { get; set; }
    public string MintERC20Amount { get; set; }

    public void MintERC20()
    {
        var mintJson = new MintERC20Payload
        {
            to = MintERC20RecipientAddress,
            amount = MintERC20Amount
        };
        string jsonString = JsonUtility.ToJson(mintJson);

        Debug.Log("Minting ERC20: " + jsonString);

        EditorApplication.delayCall += async () =>
        {
            try
            {
                string result = await sidekick.MintERC20(MintERC20ContractAddress, jsonString);
                Debug.Log("Mint result: " + result);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Mint ERC20 failed: " + ex.Message);
            }
        };
    }

    #endregion

   
    #region Mint ERC721

    public bool IsBatchMintERC721 { get; set; } = false;
    public string SafeMintERC721ContractAddress { get; set; }

    public string SafeMintERC721Recipient { get; set; }
    public string SafeMintERC721TokenId { get; set; }

    public async void SafeMintERC721()
    {
        var payload = new SafeMintERC721Payload
        {
            to = SafeMintERC721Recipient,
            tokenId = SafeMintERC721TokenId
        };
        string jsonString = JsonUtility.ToJson(payload);

        Debug.Log("Calling safeMint ERC721: " + jsonString);

        try
        {
            string result = await sidekick.SafeMintERC721(SafeMintERC721ContractAddress, jsonString);
            Debug.Log("safeMint result: " + result);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("safeMint ERC721 failed: " + ex.Message);
        }
    }

    public string[] SafeMintERC721BatchRecipients { get; set; }
    public string[] SafeMintERC721BatchTokenIds { get; set; }

    public async void SafeMintBatchERC721()
    {
        var payload = new SafeMintBatchERC721Payload
        {
            recipients = SafeMintERC721BatchRecipients,
            tokenIds = SafeMintERC721BatchTokenIds
        };
        string jsonString = JsonUtility.ToJson(payload);

        Debug.Log("Calling safeMintBatch ERC721: " + jsonString);

        try
        {
            string result = await sidekick.SafeMintBatchERC721(SafeMintERC721ContractAddress, jsonString);
            Debug.Log("safeMintBatch result: " + result);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("safeMintBatch ERC721 failed: " + ex.Message);
        }
    }

    #endregion

    #region Mint ERC1155

    public bool IsBatchMintERC1155 { get; set; } = false;

    public string MintERC1155ContractAddress { get; set; }

    public string MintERC1155RecipientAddress { get; set; }
    public string MintERC1155Id { get; set; }
    public string MintERC1155Amount { get; set; }
    public string MintERC1155Data { get; set; }
    public void MintERC1155()
    {
        var mintJson = new MintERC1155Payload
        {
            recipient = MintERC1155RecipientAddress,
            id = MintERC1155Id,
            amount = MintERC1155Amount,
            data = ToHexString(MintERC1155Data)
        };

        string jsonString = JsonUtility.ToJson(mintJson);

        Debug.Log("Minting ERC1155: " + jsonString);

        EditorApplication.delayCall += async () =>
        {
            try
            {
                string result = await sidekick.MintERC1155(MintERC1155ContractAddress, jsonString);
                Debug.Log("Mint result: " + result);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Mint ERC1155 failed: " + ex.Message);
            }
        };
    }
    public string[] MintERC1155BatchRecipients { get; set; } = new string[0];
    public string[] MintERC1155BatchIds { get; set; } = new string[0];
    public string[] MintERC1155BatchAmounts { get; set; } = new string[0];
    public string[] MintERC1155BatchDatas { get; set; } = new string[0];

    public void MintERC1155Batch()
    {
        string[] hexDatas = new string[MintERC1155BatchDatas.Length];

        for (int i = 0; i < MintERC1155BatchDatas.Length; i++)
        {
            hexDatas[i] = ToHexString(MintERC1155BatchDatas[i]);

        }

        var mintJson = new MintERC1155BatchPayload
        {
            recipients = MintERC1155BatchRecipients,
            ids = MintERC1155BatchIds,
            amounts = MintERC1155BatchAmounts,
            datas = hexDatas
        };

        string jsonString = JsonUtility.ToJson(mintJson);
        Debug.Log("Minting ERC1155 Batch: " + jsonString);

        EditorApplication.delayCall += async () =>
        {
            try
            {
                string result = await sidekick.MintBatchERC1155(MintERC1155ContractAddress, jsonString);
                Debug.Log("Mint batch result: " + result);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Mint ERC1155 batch failed: " + ex.Message);
            }
        };
    }

    #endregion


    #endregion


    #region Create SO

    public string soContractAddress { get; set; }
    public string soContractBurnAddress { get; set; }
    public int soTokenDecimals { get; set; }
    public bool soSpecifyBurnAddress { get; set; }

    #endregion

    public string ToHexString(string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        StringBuilder hex = new StringBuilder("0x");

        foreach (byte b in bytes)
        {
            hex.Append(b.ToString("x2"));
        }

        return hex.ToString();
    }
}

#region Payloads

#region Deploy

[System.Serializable]
public class DeployERC1155Payload
{
    public string defaultAdmin;
    public string minter;
    public string name;
}

[System.Serializable]
public class DeployERC20Payload
{
    public string initialOwner;
    public string name;
    public string symbol;
}

[System.Serializable]
public class DeployERC721Payload
{
    public string defaultAdmin;
    public string minter;
    public string name;
    public string symbol;
}

#endregion

#region Mint

[System.Serializable]
public class MintERC20Payload
{
    public string to;
    public string amount;
}


[System.Serializable]
public class SafeMintERC721Payload
{
    public string to;
    public string tokenId;
}

[System.Serializable]
public class SafeMintBatchERC721Payload
{
    public string[] recipients;
    public string[] tokenIds;
}
[System.Serializable]
public class MintERC1155Payload
{
    public string recipient;
    public string id;
    public string amount;
    public string data;
}

[System.Serializable]
public class MintERC1155BatchPayload
{
    public string[] recipients;
    public string[] ids;
    public string[] amounts;
    public string[] datas;
}


#endregion

#region Role

[System.Serializable]
public class GrantRolePayload
{
    public string role;
    public string account;
}

#endregion

#endregion


[System.Serializable]
public class DeployResult
{
    public ResultData result;

    [System.Serializable]
    public class ResultData
    {
        public string txHash;
        public string txUrl;
        public string error;
    }
}
