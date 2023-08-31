using System.Collections;
using UnityEngine;

namespace Sequence.Demo
{
    public class UIPanel : UIPage
    {
        public UIPage InitialPage;
        private SequenceUI _sequenceUI;

        public override void Open(params object[] args)
        {
            base.Open(args);
            if (_sequenceUI == null)
            {
                _sequenceUI = FindObjectOfType<SequenceUI>();
            }
            StartCoroutine(OpenInitialPage(args));
        }

        public override void Close()
        {
            base.Close();
            _sequenceUI.ClearStack();
        }

        public virtual IEnumerator OpenInitialPage(params object[] openArgs)
        {
            _sequenceUI.ClearStack();
            yield return new WaitForSeconds(base._openAnimationDurationInSeconds);
            yield return StartCoroutine(_sequenceUI.SetUIPage(InitialPage, openArgs));
        }
        
        public virtual void OpenWithDelay(float delayInSeconds, params object[] args)
        {
            _gameObject.SetActive(true);
            StartCoroutine(DoOpenWithDelay(delayInSeconds, args));
        }

        private IEnumerator DoOpenWithDelay(float delayInSeconds, params object[] args)
        {
            yield return new WaitForSeconds(delayInSeconds);
            Open(args);
        }
    }
}