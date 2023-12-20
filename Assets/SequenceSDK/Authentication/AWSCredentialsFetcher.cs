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
            using CognitoAWSCredentials credentials = new CognitoAWSCredentials(_identityPoolId, RegionEndpoint.GetBySystemName(_region));
            credentials.AddLogin(_issuer, _idToken);
            string identityId = await credentials.GetIdentityIdAsync();
            return identityId;
        }
        
        public async Task<Credentials> GetCredentials()
        {
            string identityId = await GetIdentityId();
            
            using CognitoAWSCredentials awsCredentials = new CognitoAWSCredentials(_identityPoolId, RegionEndpoint.GetBySystemName(_region));
            awsCredentials.AddLogin(_issuer, _idToken);
            
            GetCredentialsForIdentityRequest credentialsRequest = new GetCredentialsForIdentityRequest
            {
                IdentityId = identityId,
                Logins = new System.Collections.Generic.Dictionary<string, string>
                {
                    {_issuer, _idToken}
                }
            };
            
            using AmazonCognitoIdentityClient client = new AmazonCognitoIdentityClient(awsCredentials, RegionEndpoint.GetBySystemName(_region));
            
            GetCredentialsForIdentityResponse credentialsResponse = await client.GetCredentialsForIdentityAsync(credentialsRequest);
            Credentials credentials = credentialsResponse.Credentials;

            return credentials;
        }
        
    }

    public static class CredentialsExtentions
    {
        public static string PrettyPrint(this Credentials credentials)
        {
            return $"Access Key: {credentials.AccessKeyId}\n" +
                   $"Secret Key: {credentials.SecretKey}\n" +
                   $"Session Token: {credentials.SessionToken}\n" +
                   $"Expiration: {credentials.Expiration}";
        }
    }
}