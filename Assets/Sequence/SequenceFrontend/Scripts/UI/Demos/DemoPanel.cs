using System;
using Sequence.Utils;

namespace Sequence.Demo
{
    public abstract class DemoPanel : UIPanel
    {
        private TransitionPanel _transitionPanel;

        public override void Open(params object[] args)
        {
            base.Open(args);
            _transitionPanel = args.GetObjectOfTypeIfExists<TransitionPanel>();
            if (_transitionPanel == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(TransitionPanel)} as an argument");
            }
        }

        public override void Close()
        {
            base.Close();
            _transitionPanel.OpenWithDelay(_closeAnimationDurationInSeconds);
        }

        public override void Back(params object[] injectAdditionalParams)
        {
            if (_pageStack.Count <= 1)
            {
                Close();
            }
            else
            {
                base.Back(injectAdditionalParams);
            }
        }
    }
}