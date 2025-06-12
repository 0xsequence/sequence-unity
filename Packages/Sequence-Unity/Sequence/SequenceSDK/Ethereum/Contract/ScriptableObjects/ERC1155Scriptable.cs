using UnityEngine;
using Sequence.Contracts;

[CreateAssetMenu(fileName = "ERC1155ContractData", menuName = "Sequence/ERC1155 Contract")]
public class ERC1155Scriptable : ScriptableObject
{
    public string contractAddress;
    public bool specifyBurnAddress;

    public GameObject linkedGameObject;

    private ERC1155 _erc1155;


    public ERC1155 GetContract()
    {
        if (_erc1155 == null)
        {
            if (specifyBurnAddress)
                _erc1155 = new ERC1155(contractAddress, specifyBurnAddress);
            else
                _erc1155 = new ERC1155(contractAddress);

        }
        return _erc1155;
    }
}
