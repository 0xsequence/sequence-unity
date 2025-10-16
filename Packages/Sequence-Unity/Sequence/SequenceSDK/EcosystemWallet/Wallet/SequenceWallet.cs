using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives.Common;
using Sequence.Relayer;
using Sequence.Utils;
using UnityEngine.Assertions;

namespace Sequence.EcosystemWallet
{
    public class SequenceWallet : IWallet
    {
        public static Action<IWallet> WalletCreated;
        public static Action Disconnected;
        
        public Address Address { get; }
        
        private readonly WalletState _state;
        private SessionSigner[] _sessionSigners;
        
        internal SequenceWallet(SessionSigner[] sessionSigners)
        {
            Address = sessionSigners[0].ParentAddress;
            
            _state = new WalletState(Address);
            _sessionSigners = sessionSigners;
            
            WalletCreated?.Invoke(this);
        }

        public static IWallet RecoverFromStorage()
        {
            var credentials = SessionStorage.GetSessions();
            if (credentials == null || credentials.Length == 0)
                return null;
            
            var sessionWallets = new SessionSigner[credentials.Length];
            for (var i = 0; i < credentials.Length; i++)
                sessionWallets[i] = new SessionSigner(credentials[i]);

            return new SequenceWallet(sessionWallets);
        }

        public Address[] GetAllSigners()
        {
            return _sessionSigners.Select(x => x.Address).ToArray();
        }
        
        public async Task AddSession(IPermissions permissions)
        {
            AssertSessionSigners();
            Assert.IsNotNull(permissions);
            
            var client = new EcosystemClient();
            
            var sessionSigners = await client.CreateNewSession(true, 
                permissions.GetPermissions(), string.Empty);
            
            foreach (var sessionSigner in sessionSigners)
                _sessionSigners = _sessionSigners.AddToArray(sessionSigner);
        }
        
        public void Disconnect()
        {
            SessionStorage.Clear();
            _sessionSigners = Array.Empty<SessionSigner>();
            Disconnected?.Invoke();
        }
        
        public async Task<SignMessageResponse> SignMessage(Chain chain, string message)
        {
            AssertSessionSigners();
            
            var args = new SignMessageArgs
            { 
                address = Address, 
                chainId = new BigInt((int)chain), 
                message = message
            };
            
            var client = new EcosystemClient();
            return await client.SendRequest<SignMessageArgs, SignMessageResponse>("sign", "signMessage", args);
        }

        public async Task<FeeOption[]> GetFeeOption(Chain chain, ITransaction transaction)
        {
            return await GetFeeOption(chain, new [] { transaction });
        }
        
        public async Task<FeeOption[]> GetFeeOption(Chain chain, ITransaction[] transactions)
        {
            AssertSessionSigners();
            
            await _state.Update(chain);
            
            var calls = transactions.GetCalls();
            var txnService = new TransactionService(_sessionSigners, _state);
            var transactionData = await txnService.SignAndBuild(chain, calls, false);
            var relayer = new SequenceRelayer(chain);

            var args = new FeeOptionsArgs(transactionData.To, transactionData.To, transactionData.Data);
            var response = await relayer.GetFeeOptions(args);

            return response.options;
        }

        public async Task<string> SendTransaction(Chain chain, ITransaction transaction, FeeOption feeOption = null)
        {
            return await SendTransaction(chain, new[] { transaction }, feeOption);
        }
        
        public async Task<string> SendTransaction(Chain chain, ITransaction[] transactions, FeeOption feeOption = null)
        {
            AssertSessionSigners();
            
            await _state.Update(chain);
            
            var calls = transactions.GetCalls();

            if (feeOption != null)
            {
                var feeAddress = feeOption.token.contractAddress;
                var isNative = feeAddress == null || feeAddress.Equals(Address.ZeroAddress);
                
                var feeOptionService = new FeeOptionService(feeOption);
                var feeOptionCall = isNative
                    ? feeOptionService.BuildCallForNativeTokenOption()
                    : feeOptionService.BuildCallForCustomTokenOption();
                
                calls = calls.Unshift(feeOptionCall);
            }
            
            var txnService = new TransactionService(_sessionSigners, _state);
            var transactionData = await txnService.SignAndBuild(chain, calls, true);
                
            var relayer = new SequenceRelayer(chain);
            var hash = await relayer.Relay(transactionData.To, transactionData.Data);

            MetaTxnReceipt receipt = null;
            var status = OperationStatus.Pending;
            
            while (status != OperationStatus.Confirmed && status != OperationStatus.Failed)
            {
                var receiptResponse = await relayer.GetMetaTxnReceipt(hash);
                receipt = receiptResponse.receipt;
                status = receipt?.EvaluateStatus() ?? OperationStatus.Unknown;
            }
            
            if (receipt == null)
                throw new Exception("receipt is null");

            return receipt.txnHash;
        }
        
        public async Task<bool> SupportsTransaction(Chain chain, ITransaction transaction)
        {
            return await SupportsTransaction(chain, new[] { transaction });
        }

        public async Task<bool> SupportsTransaction(Chain chain, ITransaction[] transactions)
        {
            AssertSessionSigners();
            
            try
            {
                await _state.Update(chain);

                var calls = transactions.GetCalls();
                var signerService = new SignerService(_sessionSigners, _state.SessionsTopology);
                var signers = await signerService.FindSignersForCalls(chain, calls);
                
                return signers.Length == calls.Length;
            }
            catch (Exception e)
            {
                SequenceLog.Exception(e);
                return false;
            }
        }

        public async Task<string> SendTransactionThroughEcosystem(Chain chain, ITransaction transaction)
        {
            var chainId = BigInteger.Parse(ChainDictionaries.ChainIdOf[chain]);
            var call = transaction.GetCall();
            
            var args = new SendWalletTransactionArgs
            {
                chainId = chainId,
                address = Address,
                transactionRequest = new TransactionRequest
                {
                    to = call.to,
                    value = call.value,
                    gasLimit = call.gasLimit,
                    data = call.data.ByteArrayToHexStringWithPrefix()
                }
            };
            
            var client = new EcosystemClient();
            var response = await client.SendRequest<SendWalletTransactionArgs, SendWalletTransactionResponse>("transaction",
                "sendWalletTransaction", args);

            return response.transactionHash;
        }

        private void AssertSessionSigners()
        {
            if (_sessionSigners.Length == 0) 
                throw new Exception("No session signers available. Please sign in again.");
        }
    }
}