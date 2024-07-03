using System;
using System.Collections.Generic;
using Sequence.Config;
using Sequence.Utils.SecureStorage;
using UnityEngine;

namespace Sequence.WaaS
{
    public class WaaSSessionManager : MonoBehaviour
    {
        public static WaaSSessionManager Instance;
        
        private List<WaaSWallet> _sessions = new List<WaaSWallet>();

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
            
            WaaSWallet.OnWaaSWalletCreated += AddSession;
        }

        private void AddSession(WaaSWallet sessionWallet)
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
            WaaSWallet.OnWaaSWalletCreated -= AddSession;
            _sessions = new List<WaaSWallet>();
        }
    }
}