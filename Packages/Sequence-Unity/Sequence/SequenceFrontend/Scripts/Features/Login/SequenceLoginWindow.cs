using UnityEngine;

namespace SequenceSDK.Samples
{
    public class SequenceLoginWindow : MonoBehaviour
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
