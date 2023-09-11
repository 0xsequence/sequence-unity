using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Sequence.Authentication;
using Sequence.Demo.ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Sequence.Demo
{
    public class SequenceUI : MonoBehaviour
    {
        public static bool IsTesting = false;
        
        private LoginPanel _loginPanel;
        private TransitionPanel _transitionPanel;
        private WalletPanel _walletPanel;

        private void Awake()
        {
            _loginPanel = GetComponentInChildren<LoginPanel>();
            _transitionPanel = GetComponentInChildren<TransitionPanel>();
            _walletPanel = GetComponentInChildren<WalletPanel>();
        }

        public void Start()
        {
            if (IsTesting)
            {
                return;
            }
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