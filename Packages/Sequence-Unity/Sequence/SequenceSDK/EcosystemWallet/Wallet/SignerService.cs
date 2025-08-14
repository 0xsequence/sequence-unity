using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Sequence.EcosystemWallet
{
    internal class SignerService
    {
        private readonly SessionSigner[] _sessionSigners;
        private readonly SessionsTopology _sessions;
        
        public SignerService(SessionSigner[] sessionSigners, SessionsTopology sessions)
        {
            _sessionSigners = sessionSigners;
            _sessions = sessions;
        }
        
        public async Task<SessionSigner[]> FindSignersForCalls(Chain chain, Call[] calls)
        {
            var identitySigner = _sessions.GetIdentitySigner();
            if (identitySigner == null)
                throw new Exception("identitySigner is null");

            var blacklist = _sessions.GetImplicitBlacklist();
            if (blacklist == null)
                throw new Exception("blacklist is null");
            
            var validImplicitSigners = GetValidImplicitSigners(identitySigner, blacklist);
            var validExplicitSigners = GetValidExplicitSigners();

            var availableSigners = ArrayUtils.CombineArrays(validImplicitSigners, validExplicitSigners);
            if (availableSigners.Length == 0)
                throw new Exception("no valid signers found");
            
            var supportedSignersForCalls = await FindSignerForEachCall(chain, availableSigners, calls);
            
            if (supportedSignersForCalls.Length != calls.Length)
                throw new Exception("Unable to find a signer for one of the given calls.");
            
            return supportedSignersForCalls;
        }

        private async Task<SessionSigner[]> FindSignerForEachCall(Chain chain, SessionSigner[] availableSigners, Call[] calls)
        {
            var signers = new List<SessionSigner>();
            foreach (var call in calls)
            {
                foreach (var signer in availableSigners)
                {
                    var supported = await signer.IsSupportedCall(call, chain, _sessions);
                    if (supported)
                    {
                        signers.Add(signer);
                        break;
                    }
                }
            }
            
            return signers.ToArray();
        }

        private SessionSigner[] GetValidImplicitSigners(Address identitySigner, Address[] blacklist)
        {
            return _sessionSigners.Where(s => 
                !s.IsExplicit &&
                s.IdentitySigner.Equals(identitySigner) &&
                !blacklist.Contains(s.Address)
            ).ToArray();
        }

        private SessionSigner[] GetValidExplicitSigners()
        {
            var explicitSigners = _sessions.GetExplicitSigners();
            return _sessionSigners.Where(s =>
                s.IsExplicit && Array.Exists(explicitSigners, es => es.Equals(s.Address))).ToArray();
        }
    }
}