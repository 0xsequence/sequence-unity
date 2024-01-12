using System;
using Sequence.Config;

namespace Sequence.Authentication
{
    public class AWSConfig
    {
        public string Region { get; private set; }
        public string IdentityPoolId { get; private set; }
        public string KMSEncryptionKeyId { get; private set; }
        public string CognitoClientId { get; private set; }

        public AWSConfig(string region, string identityPoolId, string kmsEncryptionKeyId, string cognitoClientId)
        {
            if (string.IsNullOrWhiteSpace(region))
            {
                throw SequenceConfig.MissingConfigError("Region");
            }
            Region = region;
            if (string.IsNullOrWhiteSpace(identityPoolId))
            {
                throw SequenceConfig.MissingConfigError("Identity Pool Id");
            }
            IdentityPoolId = identityPoolId;
            if (string.IsNullOrWhiteSpace(kmsEncryptionKeyId))
            {
                throw SequenceConfig.MissingConfigError("KMS Encryption Key Id");
            }
            KMSEncryptionKeyId = kmsEncryptionKeyId;
            if (string.IsNullOrWhiteSpace(cognitoClientId))
            {
                throw SequenceConfig.MissingConfigError("Cognito Client Id");
            }
            CognitoClientId = cognitoClientId;
        }
    }
}