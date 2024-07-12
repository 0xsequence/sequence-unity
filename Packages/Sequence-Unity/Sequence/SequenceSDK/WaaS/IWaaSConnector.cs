using System;
using System.Threading.Tasks;
using Sequence.Authentication;
using Sequence.Utils.SecureStorage;
using SequenceSDK.WaaS;
using UnityEngine;

namespace Sequence.WaaS
{
    public interface IWaaSConnector
    {
        public Task<string> InitiateAuth(IntentDataInitiateAuth initiateAuthIntent, LoginMethod method);
        public Task ConnectToWaaS(IntentDataOpenSession loginIntent, LoginMethod method, string email = "");
    }
}