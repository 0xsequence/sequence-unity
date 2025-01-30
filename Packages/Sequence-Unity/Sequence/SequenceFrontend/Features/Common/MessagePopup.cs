using System.Collections;
using TMPro;
using UnityEngine;

namespace SequenceSDK.Samples
{
    [RequireComponent(typeof(TweenAnimation))]
    public class MessagePopup : MonoBehaviour
    {
        [SerializeField] private float _waitDuration = 3f;
        [SerializeField] private TMP_Text _messageText;
        
        [Header("Status")]
        [SerializeField] private GameObject _successStatus;
        [SerializeField] private GameObject _errorStatus;

        private TweenAnimation _tweenAnimation;
        
        public void Show(string message, bool error = false)
        {
            _tweenAnimation ??= GetComponent<TweenAnimation>();
            _tweenAnimation.PlayForward();
            
            _messageText.text = message;
            
            _successStatus.SetActive(!error);
            _errorStatus.SetActive(error);
            
            StopAllCoroutines();
            StartCoroutine(WaitRoutine());
        }

        public void Hide()
        {
            _tweenAnimation.PlayBackward();
        }

        private IEnumerator WaitRoutine()
        {
            yield return new WaitForSeconds(_waitDuration);
            Hide();
        }
    }
}
