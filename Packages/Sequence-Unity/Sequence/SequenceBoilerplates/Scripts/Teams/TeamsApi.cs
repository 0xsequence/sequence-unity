using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.EmbeddedWallet;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.Boilerplates.Teams
{
    public class TeamsApi
    {
        private readonly IWallet _wallet;
        private readonly string _teamsUrl;
        private readonly string _membersUrl;
        
        public TeamsApi(IWallet wallet, string apiUrl)
        {
            _wallet = wallet;
            _teamsUrl = $"{apiUrl}teams";
            _membersUrl = $"{apiUrl}members";
        }
        
        public async Task<TeamData[]> GetTeams()
        {
            var request = UnityWebRequest.Get(_teamsUrl);
            var success = await SendRequest(request);
            if (!success)
                throw new Exception();

            return JsonConvert.DeserializeObject<TeamData[]>(request.downloadHandler.text);
        }
        
        public async Task<TeamMemberData[]> GetMembers(string teamUid)
        {
            var request = UnityWebRequest.Get($"{_membersUrl}?teamUid={teamUid}");
            var success = await SendRequest(request);
            if (!success)
                throw new Exception();

            return JsonConvert.DeserializeObject<TeamMemberData[]>(request.downloadHandler.text);
        }

        public async Task<TeamData> GetLocalUserTeam()
        {
            var walletAddress = _wallet.GetWalletAddress();
            var request = UnityWebRequest.Get($"{_teamsUrl}?walletAddress={walletAddress}");
            var success = await SendRequest(request);
            if (!success)
                throw new Exception();

            return JsonConvert.DeserializeObject<TeamData>(request.downloadHandler.text);
        }

        public async Task<bool> CreateTeam(string name, string description, string image, string receipt)
        {
            var payload = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"name", name},
                {"description", description},
                {"image", image},
                {"receipt", receipt}
            });
            
            var request = UnityWebRequest.Get(_teamsUrl);
            request.method = UnityWebRequest.kHttpVerbPOST;
            
            return await SendRequest(request, payload);
        }
        
        public async Task<bool> UpdateTeam(string uid, string name, string description, string image)
        {
            var payload = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"uid", uid},
                {"name", name},
                {"description", description},
                {"image", image}
            });
            
            var request = UnityWebRequest.Get(_teamsUrl);
            request.method = UnityWebRequest.kHttpVerbPUT;
            
            return await SendRequest(request, payload);
        }
        
        public async Task<bool> DeleteTeam(string uid)
        {
            var payload = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"uid", uid}
            });
            
            var request = UnityWebRequest.Get(_teamsUrl);
            request.method = UnityWebRequest.kHttpVerbDELETE;
            
            return await SendRequest(request, payload);
        }
        
        public async Task<bool> JoinTeam(string teamUid, string receipt)
        {
            var payload = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"teamUid", teamUid},
                {"receipt", receipt}
            });
            
            var request = UnityWebRequest.Get(_membersUrl);
            request.method = UnityWebRequest.kHttpVerbPOST;
            
            return await SendRequest(request, payload);
        }
        
        public async Task<bool> LeaveTeam(string teamUid, string walletAddress)
        {
            var payload = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"teamUid", teamUid},
                {"walletAddress", walletAddress}
            });
            
            var request = UnityWebRequest.Get(_membersUrl);
            request.method = UnityWebRequest.kHttpVerbDELETE;
            
            return await SendRequest(request, payload);
        }
        
        public async Task<bool> PromoteUser(string teamUid, string walletAddress, bool up)
        {
            var payload = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"teamUid", teamUid},
                {"walletAddress", walletAddress},
                {"up", up}
            });
            
            var request = UnityWebRequest.Get(_membersUrl);
            request.method = UnityWebRequest.kHttpVerbPUT;
            
            return await SendRequest(request, payload);
        }

        private async Task<bool> SendRequest(UnityWebRequest request, string payload = null)
        {
            if (request.method != "GET" && payload != null)
            {
                var requestData = Encoding.UTF8.GetBytes(payload);
                request.uploadHandler = new UploadHandlerRaw(requestData);
                request.uploadHandler.contentType = "application/json";
            }

            var idToken = await _wallet.GetIdToken();
            request.SetRequestHeader("Authorization", $"Bearer {idToken.IdToken}");
            request.SetRequestHeader("Content-Type", "application/json");
            
            try
            {
                await request.SendWebRequest();
                Debug.Log(request.downloadHandler.text);
                return true;
            }
            catch (Exception e)
            {
                Debug.Log($"{e.Message} {request.downloadHandler.text}");
                return false;
            }
        }
    }
}