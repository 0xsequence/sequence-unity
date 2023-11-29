using Sequence.Demo;
using UnityEngine;

namespace Sequence.WaaS.Tests
{
    public class WaaSTestHarness : MonoBehaviour
    {
        private LoginPanel _loginPanel;
        private LoginPage _loginPage;
        
        private void Awake()
        {
            _loginPanel = GetComponentInChildren<LoginPanel>();
            _loginPage = GetComponentInChildren<LoginPage>();
        }
        
        public void Start()
        {
            DisableAllUIPages();
            OpenUIPanel(_loginPanel);
            
            _loginPage.SubscribeToWaaSWalletCreatedEvent(InitiateTests);
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

        private void InitiateTests(WaaSWallet wallet)
        {
            Debug.LogError("Wallet created. Initiating tests...");
        }
    }
}