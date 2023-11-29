using Sequence.Demo;
using UnityEngine;

namespace Sequence.WaaS.Tests
{
    public class WaaSTestHarness : MonoBehaviour
    {
        private LoginPanel _loginPanel;
        
        private void Awake()
        {
            _loginPanel = GetComponentInChildren<LoginPanel>();
        }
        
        public void Start()
        {
            DisableAllUIPages();
            OpenUIPanel(_loginPanel);
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
    }
}