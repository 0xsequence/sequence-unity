using System.Collections;
using TMPro;
using UnityEngine;

namespace SequenceSDK.Samples
{
    public class MessagePopup : MonoBehaviour
    {
        [SerializeField] private float _waitDuration = 3f;
        [SerializeField] private TMP_Text _messageText;
        
        public void Show(string message)
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            _messageText.text = message;
            
            StopAllCoroutines();
            StartCoroutine(WaitRoutine());
        }

        private IEnumerator WaitRoutine()
        {
            yield return new WaitForSeconds(_waitDuration);
            gameObject.SetActive(false);
        }
    }
}
