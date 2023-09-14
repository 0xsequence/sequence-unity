using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sequence.Demo
{
    public abstract class UIPanel : UIPage
    {
        public UIPage InitialPage;
        
        private Stack<PageWithArgs> _pageStack = new Stack<PageWithArgs>();
        protected UIPage _page;
        
        private struct PageWithArgs
        {
            public UIPage page;
            public object[] openArgs;

            public PageWithArgs(UIPage page, object[] openArgs)
            {
                this.page = page;
                this.openArgs = openArgs;
            }
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            StartCoroutine(OpenInitialPage(args));
        }

        public override void Close()
        {
            base.Close();
            ClearStack();
        }

        public virtual IEnumerator OpenInitialPage(params object[] openArgs)
        {
            ClearStack();
            yield return new WaitForSeconds(base._openAnimationDurationInSeconds);
            yield return StartCoroutine(SetUIPage(InitialPage, openArgs));
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
        
        public IEnumerator SetUIPage(UIPage page, params object[] openArgs)
        {
            if (_page != null)
            {
                _page.Close();
                yield return new WaitUntil(() => !_page.isActiveAndEnabled);
            }
            _page = page;
            _pageStack.Push(new PageWithArgs(page, openArgs));
            _page.Open(openArgs);
        }

        public virtual void Back()
        {
            if (_pageStack.Count <= 1)
            {
                return;
            }

            _pageStack.Pop().page.Close();
            PageWithArgs previous = _pageStack.Peek();
            _page = previous.page;
            _page.Open(previous.openArgs);
        }

        public void ClearStack()
        {
            _pageStack = new Stack<PageWithArgs>();
            _page = null;
        }
    }
}