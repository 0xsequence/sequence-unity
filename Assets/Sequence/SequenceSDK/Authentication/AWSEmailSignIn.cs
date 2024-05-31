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
    public class AWSEmailSignIn : IEmailSignIn
    {
        private string _region;
        private string _cognitoClientId;
        private string _nonce;
        
        public AWSEmailSignIn(string region, string cognitoClientId, string nonce)
        {
            _region = region;
            _cognitoClientId = cognitoClientId;
            _nonce = nonce;
        }
        
        /// <summary>
        /// Initiate email sign in process with AWS Cognito
        /// This will send an OTP email to the email provided
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
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
                ClientId = _cognitoClientId,
                ClientMetadata = new Dictionary<string, string>()
                {
                    { "SESSION_HASH", _nonce}
                }
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

        /// <summary>
        /// Finish email sign in process with AWS Cognito
        /// Requires the `code` given to the user via OTP email 
        /// </summary>
        /// <param name="challengeSession"></param>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <param name="sessionWalletAddress"></param>
        /// <returns></returns>
        public async Task<string> Login(string challengeSession, string email, string code, string sessionWalletAddress = "")
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
                Session = challengeSession,
                ClientMetadata = new Dictionary<string, string>()
                {
                    { "SESSION_HASH", _nonce}
                }
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

        /// <summary>
        /// Used if the user is not registered with AWS Cognito to register them
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="Exception"></exception>
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