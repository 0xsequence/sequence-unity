using System.Collections;
using UnityEngine;

namespace Sequence.Demo
{
    public class TransitionPanel : UIPanel
    {
        private SequenceUI _ui;
        private WalletPanel _walletPanel;
        protected override void Awake()
        {
            base.Awake();
            _ui = FindObjectOfType<SequenceUI>();
            _walletPanel = FindObjectOfType<WalletPanel>();
        }

        public void OpenWalletPanel()
        {
            _walletPanel.OpenWithDelay(_closeAnimationDurationInSeconds);
        }
    }
}