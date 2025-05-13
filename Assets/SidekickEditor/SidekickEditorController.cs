using UnityEditor;
using UnityEngine;
using Sequence;
using Sequence.Sidekick;

public class SidekickEditorController : MonoBehaviour
{

    private SequenceSidekickClient sidekick = new SequenceSidekickClient(Chain.TestnetArbitrumSepolia);

    #region Deploy ERC1155

    public string deployDefaultAdmin { get; set; }
    public string deployMinter { get; set; }
    public string deployName { get; set; }

    public async void DeployERC1155(string defaultAdmin, string minter, string name)
    {
        var deployPayload = new DeployERC1155Payload
        {
            defaultAdmin = defaultAdmin,
            minter = minter,
            name = name
        };
        string jsonString = JsonUtility.ToJson(deployPayload);

        Debug.Log("Deploying ERC1155:" + jsonString);

        EditorApplication.delayCall += async () =>
        {
            try
            {
                string result = await sidekick.DeployERC1155(sidekick.Chain.GetChainId(), jsonString);
                Debug.Log("Deploy result: " + result);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Deploy ERC1155 failed: " + ex.Message);
            }
        };
    }

    #endregion

    #region Mint ERC1155

    [HideInInspector] public string MintContractAddress;
    public string MintRecipientAddress { get;  set; }
    public string MintId { get; set; }
    public string MintAmount { get; set; }
    public string MintData { get; set; }

    public async void MintERC1155(string contractAddress, string recipient, string id, string amount, string data)
    {
        var mintJson = new
        {
            recipient = recipient,
            id = id,
            amount = amount,
            data = data
        };
        string jsonString = JsonUtility.ToJson(mintJson);
        Debug.Log(jsonString);

        EditorApplication.delayCall += async () =>
        {
            try
            {
                string result = await sidekick.MintERC1155(sidekick.Chain.GetChainId(), contractAddress, jsonString);
                Debug.Log("Mint result: " + result);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Mint ERC1155 failed: " + ex.Message);
            }
        };
    }

    #endregion

}
[System.Serializable]
public class DeployERC1155Payload
{
    public string defaultAdmin;
    public string minter;
    public string name;
}
