using UnityEngine;
using Sequence.Contracts;

[CreateAssetMenu(fileName = "ERC721ContractData", menuName = "Sequence/ERC721 Contract")]
public class ERC721Scriptable : ScriptableObject
{
    public string contractAddress;

    public GameObject linkedGameObject;

    private ERC721 _erc721;

    public ERC721 GetContract()
    {
        if (_erc721 == null)
        {
            _erc721 = new ERC721(contractAddress);
        }
        return _erc721;
    }
}
