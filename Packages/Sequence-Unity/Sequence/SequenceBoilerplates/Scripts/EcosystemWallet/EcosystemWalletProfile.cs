using Sequence.EcosystemWallet;
using UnityEngine;

namespace Sequence.Boilerplates
{
    public class EcosystemWalletProfile : MonoBehaviour
    {
        public void Load(IWallet wallet)
        {
            gameObject.SetActive(true);
        }
    }
}
