using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using UnityEngine;

namespace Sequence.Authentication
{
    public class AWSEmailSignIn
    {
        private string _identityPoolId;
        private string _region;
        private string _cognitoClientId;
        
        public AWSEmailSignIn(string identityPoolId, string region, string cognitoClientId)
        {
            _identityPoolId = identityPoolId;
            _region = region;
            _cognitoClientId = cognitoClientId;
        }
        
        public async Task<string> SignIn(string email)
        {
            using AmazonCognitoIdentityProviderClient client = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), RegionEndpoint.GetBySystemName(_region));
            InitiateAuthRequest request = new InitiateAuthRequest
            {
                AuthFlow = "CUSTOM_AUTH",
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", email }
                },
                ClientId = _cognitoClientId
            };
            try
            {
                InitiateAuthResponse response = await client.InitiateAuthAsync(request);
                return response.Session;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error establishing AWS session: {e.Message}");
            }

            return "";
        }

        public async Task<string> Login(string challengeSession, string email, string code)
        {
            using AmazonCognitoIdentityProviderClient client = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), RegionEndpoint.GetBySystemName(_region));
            RespondToAuthChallengeRequest request = new RespondToAuthChallengeRequest
            {
                ChallengeName = "CUSTOM_CHALLENGE",
                ChallengeResponses = new Dictionary<string, string>
                {
                    { "USERNAME", email },
                    { "ANSWER", code }
                },
                ClientId = _cognitoClientId,
                Session = challengeSession
            };
            
            try
            {
                RespondToAuthChallengeResponse response = await client.RespondToAuthChallengeAsync(request);
                return response.AuthenticationResult.IdToken;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error establishing AWS session: {e.Message}");
            }

            return "";
        }
    }
}