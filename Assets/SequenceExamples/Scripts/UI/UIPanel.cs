using System.Collections;
using UnityEngine;

namespace Sequence.Demo
{
    public class UIPanel : UIPage
    {
        public UIPage InitialPage;

        public override void Open(params object[] args)
        {
            base.Open(args);
            StartCoroutine(OpenInitialPage(args));
        }
        
        public virtual IEnumerator OpenInitialPage(params object[] openArgs)
        {
            yield return new WaitForSeconds(base._openAnimationDurationInSeconds);
            InitialPage.Open(openArgs);
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