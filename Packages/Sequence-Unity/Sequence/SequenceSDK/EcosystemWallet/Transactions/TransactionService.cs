using System.Numerics;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Envelope;
using Sequence.EcosystemWallet.Primitives;
using Sequence.EcosystemWallet.Utils;
using Sequence.Utils;

namespace Sequence.EcosystemWallet
{
    internal class TransactionService
    {
        private readonly SessionSigner[] _sessionSigners;
        private readonly WalletState _state;
        
        public TransactionService(SessionSigner[] sessionSigners, WalletState state)
        {
            _sessionSigners = sessionSigners;
            _state = state;
        }
        
        public async Task<(Address To, string Data)> SignAndBuild(Chain chain, Call[] calls, bool checkDeployed)
        {
            var preparedIncrement = PrepareIncrement(null, 0, null);
            if (preparedIncrement != null)
                calls.AddToArray(preparedIncrement);

            var envelope = PrepareTransaction(chain, calls);

            var signatureService = new SignatureService(_sessionSigners, _state.SessionsTopology);
            var signature = await signatureService.SignCalls(chain, _state.SessionsImageHash, envelope, _state.ConfigUpdates);

            var callsData = ABI.ABI.Pack("execute(bytes,bytes)",
                envelope.payload.Encode(),
                signature.Encode());
            
            if (!checkDeployed || _state.IsDeployed)
            {
                return (_state.Address, callsData);
            }

            // Not relevant for signing calls for getting fee options
            // If the wallet was not yet deployed onchain, let's make a deploy transaction first
            
            var deployTransaction = Erc6492Helper.Deploy(_state.DeployHash, new Erc6492Helper.Context
            {
                creationCode = _state.DeployContext.walletCreationCode,
                factory = _state.DeployContext.factory,
                stage1 = _state.DeployContext.mainModule,
                stage2 = _state.DeployContext.mainModuleUpgradable
            });
            
            return (new Address(_state.DeployContext.guestModule), new Calls(0, 0, new Call[]
            {
                new (deployTransaction.To, 0, deployTransaction.Data.HexStringToByteArray()),
                new (_state.Address, 0, callsData.HexStringToByteArray())
            }).Encode().ByteArrayToHexStringWithPrefix());
        }
        
        private Call PrepareIncrement(Address wallet, BigInteger chainId, Calls calls)
        {
            // TODO: Integrate increments
            return null;
        }

        private Envelope<Calls> PrepareTransaction(Chain chain, Call[] calls)
        {
            return new Envelope<Calls>
            {
                chainId = BigInteger.Parse(ChainDictionaries.ChainIdOf[chain]),
                wallet = _state.Address,
                configuration = _state.Config,
                payload = new Calls(0, _state.Nonce, calls)
            };
        }
    }
}