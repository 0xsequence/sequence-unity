using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

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
                return $"Error establishing AWS session: {e.Message}";
            }
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
                if (response.AuthenticationResult == null)
                {
                    return "Error establishing session: Incorrect code";
                }
                return response.AuthenticationResult.IdToken;
            }
            catch (Exception e)
            {
                return $"Error establishing AWS session: {e.Message}";
            }
        }

        public async Task SignUp(string email)
        {
            using AmazonCognitoIdentityProviderClient client = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), RegionEndpoint.GetBySystemName(_region));
            SignUpRequest request = new SignUpRequest
            {
                ClientId = _cognitoClientId,
                Username = email,
                Password = GeneratePassword(),
                UserAttributes = new List<AttributeType>
                {
                    new AttributeType
                    {
                        Name = "email",
                        Value = email
                    }
                }
            };

            try
            {
                SignUpResponse response = await client.SignUpAsync(request);
                int i = 0;
            }
            catch (Exception e)
            {
                throw new Exception($"Error establishing AWS session: {e.Message}");
            }
        }

        private string GeneratePassword()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_-+=";

            StringBuilder randomChars = new StringBuilder(12);
            for (int i = 0; i < 12; i++)
            {
                randomChars.Append(chars[Random.Range(0, chars.Length)]);
            }

            string password = $"aB1%{randomChars}";

            return password;
        }
    }
}