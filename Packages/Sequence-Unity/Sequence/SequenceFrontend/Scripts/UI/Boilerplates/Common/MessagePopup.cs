using System.Collections;
using Sequence.Demo.Tweening;
using TMPro;
using UnityEngine;

namespace Sequence.Boilerplates
{
    [RequireComponent(typeof(ITween))]
    public class MessagePopup : MonoBehaviour
    {
        [SerializeField] private float _waitDuration = 3f;
        [SerializeField] private TMP_Text _messageText;
        
        [Header("Status")]
        [SerializeField] private GameObject _successStatus;
        [SerializeField] private GameObject _errorStatus;

        private ITween _tweenAnimation;
        
        public void Show(string message, bool error = false)
        {
            _tweenAnimation ??= GetComponent<ITween>();
            _tweenAnimation.AnimateIn(0.3f);
            
            _messageText.text = message;
            
            _successStatus.SetActive(!error);
            _errorStatus.SetActive(error);
            
            StopAllCoroutines();
            StartCoroutine(WaitRoutine());
        }

        public void Hide()
        {
            _tweenAnimation.AnimateOut(0.3f);
        }

        private IEnumerator WaitRoutine()
        {
            yield return new WaitForSeconds(_waitDuration);
            Hide();
        }
    }
}
