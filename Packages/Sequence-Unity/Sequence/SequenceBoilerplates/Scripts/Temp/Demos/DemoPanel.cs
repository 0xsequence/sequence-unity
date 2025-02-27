using System;
using Sequence.Boilerplates;
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
        }

        public override void Close()
        {
            base.Close();

            if (_transitionPanel != null)
            {
                _transitionPanel.OpenWithDelay(_closeAnimationDurationInSeconds);
            }
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