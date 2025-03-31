using System;
using System.Collections;
using System.Collections.Generic;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Demo
{
    public abstract class UIPanel : UIPage
    {
        public UIPage InitialPage;
        
        protected Stack<PageWithArgs> _pageStack = new Stack<PageWithArgs>();
        protected UIPage _page;
        protected bool _isOpen = false;
        protected Action _onClose;
        
        protected struct PageWithArgs
        {
            public UIPage page;
            public object[] openArgs;

            public PageWithArgs(UIPage page, object[] openArgs)
            {
                this.page = page;
                this.openArgs = openArgs;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _panel = this;
            Debug.Log(this);
        }

#if ENABLE_LEGACY_INPUT_MANAGER
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GoBack();
            }
        }
#endif

        public override void Open(params object[] args)
        {
            _gameObject.SetActive(true);
            _animator.AnimateIn( _openAnimationDurationInSeconds);
            _onClose = args.GetObjectOfTypeIfExists<Action>();
            StartCoroutine(OpenInitialPage(args));
            _isOpen = true;
        }

        public override void Close()
        {
            base.Close();
            ClearStack();
            _isOpen = false;

            _onClose?.Invoke();
        }

        public virtual IEnumerator OpenInitialPage(params object[] openArgs)
        {
            if (InitialPage != null && InitialPage != _page)
            {
                ClearStack();
                yield return new WaitForSeconds(base._openAnimationDurationInSeconds);
                yield return StartCoroutine(SetUIPage(InitialPage, openArgs));
            }
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
        
        public virtual IEnumerator SetUIPage(UIPage page, params object[] openArgs)
        {
            if (_page != null)
            {
                _page.Close();
                yield return new WaitUntil(() => !_page.isActiveAndEnabled);
            }
            _page = page;
            _pageStack.Push(new PageWithArgs(page, openArgs));
            _page.Open(openArgs.AppendObject(this));
        }

        protected void OpenPageOverlaid(UIPage page, params object[] openArgs)
        {
            _page = page;
            _pageStack.Push(new PageWithArgs(page, openArgs));
            page.Open(openArgs.AppendObject(this));
        }

        public virtual void Back(params object[] injectAdditionalParams)
        {
            Debug.Log($"Go Back, stack: {_pageStack.Count}");
            if (_pageStack.Count <= 1)
            {
                return;
            }

            _pageStack.Pop().page.Close();
            PageWithArgs previous = _pageStack.Peek();
            _page = previous.page;
            _page.Open(previous.openArgs.AppendArray(injectAdditionalParams.AppendObject(this)));
        }

        public void GoBack()
        {
            _page.Back();
        }

        public void ClearStack()
        {
            _pageStack = new Stack<PageWithArgs>();
            if (_page != null)
            {
                _page.Close();
            }
            _page = null;
        }

        public float GetCloseAnimationDuration()
        {
            return _closeAnimationDurationInSeconds;
        }

        public bool IsOpen()
        {
            return _isOpen;
        }
    }
}