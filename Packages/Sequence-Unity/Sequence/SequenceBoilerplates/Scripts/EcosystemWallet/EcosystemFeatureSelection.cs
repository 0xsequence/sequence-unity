using UnityEngine;

namespace Sequence.Boilerplates
{
    public class EcosystemFeatureSelection : MonoBehaviour
    {
        [SerializeField] private EcosystemWalletProfile _profile;
        [SerializeField] private EcosystemWalletTransactions _transactions;

        public void OpenProfile()
        {
            _profile.Load(null);
        }

        public void OpenTransactions()
        {
            _transactions.Load(null);
        }
    }
}
