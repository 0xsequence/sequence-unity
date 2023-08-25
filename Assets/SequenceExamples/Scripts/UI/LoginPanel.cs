using System.Collections;
using UnityEngine;

namespace Sequence.Demo
{
    public class LoginPanel : UIPanel
    {
        private SequenceUI _ui;
        private TransitionPanel _transitionPanel;
        protected override void Awake()
        {
            base.Awake();
            _ui = FindObjectOfType<SequenceUI>();
            _transitionPanel = FindObjectOfType<TransitionPanel>();
        }

        public void OpenTransitionPanel()
        {
            _transitionPanel.OpenWithDelay(_closeAnimationDurationInSeconds);
        }
    }
}