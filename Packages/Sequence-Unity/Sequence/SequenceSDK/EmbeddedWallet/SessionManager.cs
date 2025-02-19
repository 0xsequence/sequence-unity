using System;
using System.Collections.Generic;
using Sequence.Config;
using Sequence.Utils.SecureStorage;
using UnityEngine;

namespace Sequence.EmbeddedWallet
{
    public class SessionManager : MonoBehaviour
    {
        public static SessionManager Instance;
        
        private List<SequenceWallet> _sessions = new List<SequenceWallet>();

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
            
            SequenceWallet.OnWalletCreated += AddSession;
        }

        private void AddSession(SequenceWallet sessionWallet)
        {
            _sessions.Add(sessionWallet);
        }

        private void OnApplicationQuit()
        {
            if (SequenceConfig.GetConfig(SequenceService.WaaS).StoreSessionKey() && SecureStorageFactory.IsSupportedPlatform())
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
            SequenceWallet.OnWalletCreated -= AddSession;
            _sessions = new List<SequenceWallet>();
        }
    }
}