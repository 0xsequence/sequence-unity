using System;
using Sequence.Boilerplates;
using Sequence.Utils;

namespace Sequence.Demo
{
    public abstract class DemoPanel : UIPanel
    {
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