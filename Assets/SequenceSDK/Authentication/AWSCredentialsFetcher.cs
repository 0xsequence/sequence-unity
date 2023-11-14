using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using UnityEngine;

namespace Sequence.Authentication
{
    public class AWSCredentialsFetcher
    {
        private string _idToken;
        private string _identityPoolId;
        private string _region;
        private string _issuer;

        public AWSCredentialsFetcher(string idToken, string identityPoolId, string region)
        {
            _idToken = idToken;
            _identityPoolId = identityPoolId;
            _region = region;
            _issuer = ExtractIssFromJwt(idToken);
        }

        private string ExtractIssFromJwt(string jwt)
        {
            IdTokenJwtPayload payload = JwtHelper.GetIdTokenJwtPayload(jwt);
            return payload.iss.Replace("https://", "").Replace("http://", "");
        }

        private async Task<string> GetIdentityId()
        {
            GetIdRequest idRequest = new GetIdRequest
            {
                IdentityPoolId = _identityPoolId,
                Logins = new System.Collections.Generic.Dictionary<string, string>
                {
                    {_issuer, _idToken}
                }
            };
            
            using AmazonCognitoIdentityClient client = new AmazonCognitoIdentityClient(new AmazonCognitoIdentityConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_region)
            });
            GetIdResponse idResponse = await client.GetIdAsync(idRequest);
            string identityId = idResponse.IdentityId;
            return identityId;
        }
        
        public async Task<Credentials> GetCredentials()
        {
            string identityId = await GetIdentityId();
            
            GetCredentialsForIdentityRequest credentialsRequest = new GetCredentialsForIdentityRequest
            {
                IdentityId = identityId,
                Logins = new System.Collections.Generic.Dictionary<string, string>
                {
                    {_issuer, _idToken}
                }
            };
            
            using AmazonCognitoIdentityClient client = new AmazonCognitoIdentityClient();
            
            GetCredentialsForIdentityResponse credentialsResponse = await client.GetCredentialsForIdentityAsync(credentialsRequest);
            Credentials credentials = credentialsResponse.Credentials;

            return credentials;
        }
    }
}