using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity.Model;
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;

namespace Sequence.Authentication
{
    public class AWSDataKeyGenerator
    {
        private Credentials _credentials;
        private string _region;
        private string _kmsEncryptionKeyId;
        
        public AWSDataKeyGenerator(Credentials credentials, string region, string kmsEncryptionKeyId)
        {
            _credentials = credentials;
            _region = region;
            _kmsEncryptionKeyId = kmsEncryptionKeyId;
        }
        
        public async Task<DataKey> GenerateDataKey()
        {
            using AmazonKeyManagementServiceClient client = new AmazonKeyManagementServiceClient(_credentials, RegionEndpoint.GetBySystemName(_region));
            
            GenerateDataKeyRequest request = new GenerateDataKeyRequest
            {
                KeyId = _kmsEncryptionKeyId,
                KeySpec = DataKeySpec.AES_256
            };
            
            GenerateDataKeyResponse response = await client.GenerateDataKeyAsync(request);
            DataKey dataKey = new DataKey(
                response.Plaintext.ToArray(), 
                response.CiphertextBlob.ToArray());
            return dataKey;
        }
    }

    public class DataKey
    {
        public byte[] Plaintext { get; private set; }
        public byte[] Ciphertext { get; private set; }

        public DataKey(byte[] plaintext, byte[] ciphertext)
        {
            Plaintext = plaintext;
            Ciphertext = ciphertext;
        }
    }
}