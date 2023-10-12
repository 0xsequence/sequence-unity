using Sequence.Demo.ScriptableObjects;
using UnityEngine;

namespace Sequence.Demo
{
    public abstract class WalletUIElement : MonoBehaviour
    {
        public NetworkIcons NetworkIcons;
        public ITransactionDetailsFetcher TransactionDetailsFetcher = new MockTransactionDetailsFetcher(15); // Todo: replace mock with concrete implementation

        public abstract Chain GetNetwork();
    }
}