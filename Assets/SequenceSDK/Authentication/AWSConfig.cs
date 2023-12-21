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
            Region = region;
            IdentityPoolId = identityPoolId;
            KMSEncryptionKeyId = kmsEncryptionKeyId;
            CognitoClientId = cognitoClientId;
        }
    }
}