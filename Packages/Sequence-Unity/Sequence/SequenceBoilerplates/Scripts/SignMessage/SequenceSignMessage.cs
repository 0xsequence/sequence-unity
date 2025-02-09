using UnityEngine;

namespace Sequence.Boilerplates.SignMessage
{
    public class SequenceSignMessage : MonoBehaviour
    {
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}
