using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.Authentication;
using Sequence.Wallet;

namespace Sequence.WaaS
{
    public class WaaSWallet : IWallet
    {
        private Address _address;
        private HttpClient _httpClient;
        private IntentSender _intentSender;

        public WaaSWallet(Address address, string sessionId, EthWallet sessionWallet, DataKey awsDataKey, int waasProjectId, string waasVersion)
        {
            _address = address;
            _httpClient = new HttpClient("https://d14tu8valot5m0.cloudfront.net/rpc/WaasWallet");
            _intentSender = new IntentSender(new HttpClient("https://d14tu8valot5m0.cloudfront.net/rpc/WaasAuthenticator"), awsDataKey, sessionWallet, sessionId, waasProjectId, waasVersion);
        }

        public Task<CreatePartnerReturn> CreatePartner(CreatePartnerArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<CreatePartnerArgs, CreatePartnerReturn>("CreatePartner", args, headers);
        }

        public Task<UpdatePartnerReturn> UpdatePartner(UpdatePartnerArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<UpdatePartnerArgs, UpdatePartnerReturn>("UpdatePartner", args, headers);
        }

        public Task<CreatePartnerWalletConfigReturn> CreatePartnerWalletConfig(CreatePartnerWalletConfigArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<CreatePartnerWalletConfigArgs, CreatePartnerWalletConfigReturn>("CreatePartnerWalletConfig", args, headers);
        }

        public Task<PartnerWalletConfigReturn> PartnerWalletConfig(PartnerWalletConfigArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<PartnerWalletConfigArgs, PartnerWalletConfigReturn>("PartnerWalletConfig", args, headers);
        }

        public Task<DeployPartnerParentWalletReturn> DeployPartnerParentWallet(DeployPartnerParentWalletArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<DeployPartnerParentWalletArgs, DeployPartnerParentWalletReturn>("DeployPartnerParentWallet", args, headers);
        }

        public Task<AddPartnerWalletSignerReturn> AddPartnerWalletSigner(AddPartnerWalletSignerArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<AddPartnerWalletSignerArgs, AddPartnerWalletSignerReturn>("AddPartnerWalletSigner", args, headers);
        }

        public Task<RemovePartnerWalletSignerReturn> RemovePartnerWalletSigner(RemovePartnerWalletSignerArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<RemovePartnerWalletSignerArgs, RemovePartnerWalletSignerReturn>("RemovePartnerWalletSigner", args, headers);
        }

        public Task<ListPartnerWalletSignersReturn> ListPartnerWalletSigners(ListPartnerWalletSignersArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<ListPartnerWalletSignersArgs, ListPartnerWalletSignersReturn>("ListPartnerWalletSigners", args, headers);
        }

        public Task<PartnerWalletsReturn> PartnerWallets(PartnerWalletsArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<PartnerWalletsArgs, PartnerWalletsReturn>("PartnerWallets", args, headers);
        }

        public Task<CreateWalletReturn> CreateWallet(CreateWalletArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<CreateWalletArgs, CreateWalletReturn>("CreateWallet", args, headers);
        }

        public Task<GetWalletAddressReturn> GetWalletAddress(GetWalletAddressArgs args)
        {
            GetWalletAddressReturn result = new GetWalletAddressReturn(this._address.Value);
            return Task.FromResult(result);
        }

        public Task<DeployWalletReturn> DeployWallet(DeployWalletArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<DeployWalletArgs, DeployWalletReturn>("DeployWallet", args, headers);
        }

        public Task<WalletsReturn> Wallets(WalletsArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<WalletsArgs, WalletsReturn>("Wallets", args, headers);
        }

        public event Action<SignMessageReturn> OnSignMessageComplete;

        public async Task<SignMessageReturn> SignMessage(SignMessageArgs args)
        {
            var result = await _intentSender.SendIntent<SignMessageReturn, SignMessageArgs>(args);
            OnSignMessageComplete?.Invoke(result);
            return result;
        }

        public Task<IsValidMessageSignatureReturn> IsValidMessageSignature(IsValidMessageSignatureArgs args)
        {
            return _intentSender.SendIntent<IsValidMessageSignatureReturn, IsValidMessageSignatureArgs>(args);
        }

        public event Action<SuccessfulTransactionReturn[]> OnSendTransactionComplete;

        public Task<SuccessfulTransactionReturn> SendTransaction(SendTransactionArgs args)
        {
            return _intentSender.SendIntent<SuccessfulTransactionReturn, SendTransactionArgs>(args);
        }
    }
}