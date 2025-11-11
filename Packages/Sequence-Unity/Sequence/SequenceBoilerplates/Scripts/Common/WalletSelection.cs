using UnityEngine;

namespace Sequence.Boilerplates
{
    public class WalletSelection : MonoBehaviour
    {
        public void OpenEcosystemWalletLogin()
        {
            gameObject.SetActive(false);
            BoilerplateFactory.OpenEcosystemWalletLogin(transform.parent, () => gameObject.SetActive(true));
        }
        
        public void OpenEmbeddedWalletLogin()
        {
            gameObject.SetActive(false);
            BoilerplateFactory.OpenEmbeddedWalletLogin(transform.parent, () => gameObject.SetActive(true));
        }
    }
}
