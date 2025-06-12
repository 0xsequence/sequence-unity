using UnityEngine;
using Sequence.Contracts;

[CreateAssetMenu(fileName = "ERC20ContractData", menuName = "Sequence/ERC20 Contract")]
public class ERC20Scriptable : ScriptableObject
{
    public string contractAddress;

    public GameObject linkedGameObject;

    private ERC20 _erc20;

    public ERC20 GetContract()
    {
        if (_erc20 == null)
        {
            _erc20 = new ERC20(contractAddress);
        }
        return _erc20;
    }
}
