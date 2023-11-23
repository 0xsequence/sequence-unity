using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sequence.WaaS
{   
    public interface IWallet
    {
        public Task<CreatePartnerReturn> CreatePartner(CreatePartnerArgs args,
            Dictionary<string, string> headers = null);
        
        public Task<UpdatePartnerReturn> UpdatePartner(UpdatePartnerArgs args,
            Dictionary<string, string> headers = null);

        public Task<CreatePartnerWalletConfigReturn> CreatePartnerWalletConfig(CreatePartnerWalletConfigArgs args,
            Dictionary<string, string> headers = null);

        public Task<PartnerWalletConfigReturn> PartnerWalletConfig(PartnerWalletConfigArgs args,
            Dictionary<string, string> headers = null);

        public Task<DeployPartnerParentWalletReturn> DeployPartnerParentWallet(DeployPartnerParentWalletArgs args,
            Dictionary<string, string> headers = null);

        public Task<AddPartnerWalletSignerReturn> AddPartnerWalletSigner(AddPartnerWalletSignerArgs args,
            Dictionary<string, string> headers = null);

        public Task<RemovePartnerWalletSignerReturn> RemovePartnerWalletSigner(RemovePartnerWalletSignerArgs args,
            Dictionary<string, string> headers = null);

        public Task<ListPartnerWalletSignersReturn> ListPartnerWalletSigners(ListPartnerWalletSignersArgs args,
            Dictionary<string, string> headers = null);

        public Task<PartnerWalletsReturn> PartnerWallets(PartnerWalletsArgs args,
            Dictionary<string, string> headers = null);

        public Task<CreateWalletReturn> CreateWallet(CreateWalletArgs args, Dictionary<string, string> headers = null);

        public Task<GetWalletAddressReturn> GetWalletAddress(GetWalletAddressArgs args);

        public Task<DeployWalletReturn> DeployWallet(DeployWalletArgs args, Dictionary<string, string> headers = null);
        
        public Task<WalletsReturn> Wallets(WalletsArgs args, Dictionary<string, string> headers = null);

        public event Action<SignMessageReturn> OnSignMessageComplete;
        public Task<SignMessageReturn> SignMessage(SignMessageArgs args);

        public Task<IsValidMessageSignatureReturn> IsValidMessageSignature(IsValidMessageSignatureArgs args);
        public event Action<SuccessfulTransactionReturn> OnSendTransactionComplete;
        public event Action<FailedTransactionReturn> OnSendTransactionFailed;
        public Task<TransactionReturn> SendTransaction(SendTransactionArgs args);
        public event Action<string> OnDropSessionComplete;
        public Task<bool> DropSession(string dropSessionId);
        public Task<bool> DropThisSession();
    }
}