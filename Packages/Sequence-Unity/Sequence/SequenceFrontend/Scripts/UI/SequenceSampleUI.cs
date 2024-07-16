using Sequence.WaaS;
using UnityEngine;

namespace Sequence.Demo
{
    public class SequenceSampleUI : MonoBehaviour
    {
        #region TestConfig
        public static bool IsTesting = false;
        public static UIPanel InitialPanel;
        public static object[] InitialPanelOpenArgs;
        #endregion
        
        private LoginPanel _loginPanel;
        private TransitionPanel _transitionPanel;
        private WalletPanel _walletPanel;
        private SignMessagePanel _signMessagePanel;
        private SendTransactionPanel _sendTransactionPanel;
        private SendTransactionWithFeeOptionsPanel _sendTransactionWithFeeOptionsPanel;

        private void Awake()
        {
            _loginPanel = GetComponentInChildren<LoginPanel>();
            _transitionPanel = GetComponentInChildren<TransitionPanel>();
            _walletPanel = GetComponentInChildren<WalletPanel>();
            _signMessagePanel = GetComponentInChildren<SignMessagePanel>();
            _sendTransactionPanel = GetComponentInChildren<SendTransactionPanel>();
            _sendTransactionWithFeeOptionsPanel = GetComponentInChildren<SendTransactionWithFeeOptionsPanel>();

            if (!IsTesting)
            {
                InitialPanel = _loginPanel;
            }

            WaaS.SequenceWallet.OnWalletCreated += wallet =>
            {
                wallet.OnDropSessionComplete += s =>
                {
                    if (s == wallet.SessionId)
                    {
                        ReplaceWithLoginPanel();
                    }
                };
            };
        }

        public void Start()
        {
            if (IsTesting)
            {
                return;
            }
            DisableAllUIPages();
            OpenUIPanel(InitialPanel, InitialPanelOpenArgs);
        }

        private void DisableAllUIPages()
        {
            UIPage[] pages = GetComponentsInChildren<UIPage>();
            int count = pages.Length;
            for (int i = 0; i < count; i++)
            {
                pages[i].gameObject.SetActive(false);
            }
        }

        private void OpenUIPanel(UIPanel panel, params object[] openArgs)
        {
            panel.Open(openArgs);
        }

        public void OpenWalletPanelWithDelay(float delayInSeconds, params object[] openArgs)
        {
            _walletPanel.OpenWithDelay(delayInSeconds, openArgs);
        }
        
        public void OpenSignMessagePanelWithDelay(float delayInSeconds, params object[] openArgs)
        {
            _signMessagePanel.OpenWithDelay(delayInSeconds, openArgs);
        }
        
        public void OpenSendTransactionPanelWithDelay(float delayInSeconds, params object[] openArgs)
        {
            _sendTransactionPanel.OpenWithDelay(delayInSeconds, openArgs);
        }
        
        public void OpenSendTransactionWithFeeOptionsPanelWithDelay(float delayInSeconds, params object[] openArgs)
        {
            _sendTransactionWithFeeOptionsPanel.OpenWithDelay(delayInSeconds, openArgs);
        }

        private void ReplaceWithLoginPanel()
        {
            float delayInSeconds = 0;
            if (_transitionPanel != null && _transitionPanel.IsOpen())
            {
                _transitionPanel.Close();
                delayInSeconds = _transitionPanel.GetCloseAnimationDuration();
            }
            if (_signMessagePanel != null && _signMessagePanel.IsOpen())
            {
                _signMessagePanel.Close();
                delayInSeconds = _signMessagePanel.GetCloseAnimationDuration();
            }
            if (_sendTransactionPanel != null && _sendTransactionPanel.IsOpen())
            {
                _sendTransactionPanel.Close();
                delayInSeconds = _sendTransactionPanel.GetCloseAnimationDuration();
            }
            if (_walletPanel != null && _walletPanel.IsOpen())
            {
                _walletPanel.Close();
                delayInSeconds = _walletPanel.GetCloseAnimationDuration();
            }
            
            _loginPanel.OpenWithDelay(delayInSeconds, true);
        }
    }
}