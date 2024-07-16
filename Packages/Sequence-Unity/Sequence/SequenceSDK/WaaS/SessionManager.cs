using System;
using System.Collections.Generic;
using Sequence.Config;
using Sequence.Utils.SecureStorage;
using UnityEngine;

namespace Sequence.WaaS
{
    public class SessionManager : MonoBehaviour
    {
        public static SessionManager Instance;
        
        private List<Wallet> _sessions = new List<Wallet>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            Wallet.OnWalletCreated += AddSession;
        }

        private void AddSession(Wallet sessionWallet)
        {
            _sessions.Add(sessionWallet);
        }

        private void OnApplicationQuit()
        {
            if (SequenceConfig.GetConfig().StoreSessionPrivateKeyInSecureStorage && SecureStorageFactory.IsSupportedPlatform())
            {
                return;
            }
            
            int sessionsCount = _sessions.Count;
            for (int i = 0; i < sessionsCount; i++)
            {
                _sessions[i].DropThisSession();
            }
        }

        private void OnDestroy()
        {
            Wallet.OnWalletCreated -= AddSession;
            _sessions = new List<Wallet>();
        }
    }
}