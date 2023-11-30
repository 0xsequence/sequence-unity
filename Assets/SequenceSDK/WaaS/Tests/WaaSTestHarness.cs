using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.Demo;
using UnityEngine;

namespace Sequence.WaaS.Tests
{
    public class WaaSTestHarness : MonoBehaviour
    {
        public static Action<WaaSTestFailed> TestFailed;
        public static Action TestPassed;
        public static Action TestStarted;

        private LoginPanel _loginPanel;
        private LoginPage _loginPage;

        private List<string> _failedTests = new List<string>();
        private int _passedTests = 0;
        private int _testsStarted = 0;
        
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
            TestFailed += (failed) =>
            {
                _failedTests.Add(failed.ToString());
                Debug.LogError(failed.ToString());
            };
            TestPassed += () => _passedTests++;
            TestStarted += () => _testsStarted++;
            WaaSWalletTests walletTests = new WaaSWalletTests(wallet);
            RunTests(walletTests);
        }

        private async Task RunTests(WaaSWalletTests walletTests)
        {
            walletTests.TestMessageSigning("Hello world", Chain.Polygon);
            walletTests.TestTransfer();
            walletTests.TestSendERC20();
            await Task.Delay(100);
            while (_testsStarted > _failedTests.Count + _passedTests)
            {
                await Task.Delay(100);
            }
            
            Debug.LogError($"Tests run: {_testsStarted} | Tests passed: {_passedTests} | Tests failed: {_failedTests.Count}");
        }
    }

    public class WaaSTestFailed
    {
        public string Name;
        public object[] Args;
        
        public WaaSTestFailed(string name, params object[] args)
        {
            Name = name;
            Args = args;
        }
        
        public override string ToString()
        {
            return $"Test {Name} failed with args: {string.Join(", ", Args)}";
        }
    }
}