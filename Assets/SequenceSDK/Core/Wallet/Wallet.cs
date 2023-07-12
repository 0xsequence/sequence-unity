#define HAS_SPAN
#define SECP256K1_LIB

using Sequence.Core.Provider;
using Sequence.Wallet;
using System.Collections.Generic;
using System.Numerics;
using Sequence.Core;

namespace Sequence.Core.Wallet
{

    public class NetworkConfig
    {
        public string Name { get; set; }
        public BigInteger ChainID { get; set; }
        public string ENSAddress { get; set; }
        public string RpcURL { get; set; }
/*        public ethrpc.Provider Provider { get; set; }
        public string RelayerURL { get; set; }
        public Relayer Relayer { get; set; }*/
        public string IndexerURL { get; set; }
        // public Indexer Indexer { get; set; }
        public bool IsDefaultChain { get; set; }
        public bool IsAuthChain { get; set; }
        public string SequenceAPIURL { get; set; }
    }



    //================================================
    public class WalletOptions
    {
        // Config is the wallet multi-sig configuration. Note: the first config of any wallet
        // before it is deployed is used to derive it's the account address of the wallet.
        public Config Config { get; set; }
        // Context is the WalletContext of deployed wallet-contract modules for the Smart Wallet.
        // NOTE: if a WalletContext is not provided, then `SequenceContext()` value is used.
        public WalletContext Context { get; set; } = Wallet.sequenceContextV2;
        // Skips config sorting and keeps signers order as-is
        public bool SkipSortSigners { get; set; }
        // Address used for the wallet
        // if this value is defined, the address derived from the sequence config is ignored
        public string Address { get; set; }

    }

    public class Wallet
    {
        
        
        public WalletContext context { get; set; }
        public Config config { get; set; }
        public List<IWallet> signers { get; set; } //EOA signers

        public RPCProvider provider { get; set; } //eth provider
        public WalletProvider walletProvider { get; set; }
        //TODO: Relayer
        //public Relayer relayer;
        public BigInteger chainID { get; set; }
        public string Address { get; set; }
        public bool SkipSortSigners { get; set; }



        // Without Relayer

        public Wallet(WalletContext context, Config config, List<IWallet> signers, RPCProvider provider,  string address, bool skipSortSigners, BigInteger chainID)
        {
            this.context = context;
            this.config = config;
            this.signers = signers;
            this.provider = provider;
            this.Address = address;
            this.SkipSortSigners = skipSortSigners;
            this.chainID = chainID;
        }

        


        //Switch to NBitcoin.Secp256k1 functions
        //TODO: Testing, will refactor later


        public static WalletContext sequenceContextV2
        = new WalletContext
        {
            FactoryAddress = "0xFaA5c0b14d1bED5C888Ca655B9a8A5911F78eF4A",
            MainModuleAddress = "0xfBf8f1A5E00034762D928f46d438B947f5d4065d",
            MainModuleUpgradableAddress = "0x4222dcA3974E39A8b41c411FeDDE9b09Ae14b911",
            GuestModuleAddress = "0xfea230Ee243f88BC698dD8f1aE93F8301B6cdfaE"
        };

        

        

        public string RecoverAddressFromDigest()
        {
            throw new System.NotImplementedException();
        }

        public void Transactor(Wallet wallet)
        {

            throw new System.NotImplementedException();
        }

        public void TransactorForChainID(Wallet wallet)
        {
            throw new System.NotImplementedException();
        }

        public void GetProvider()
        {
            throw new System.NotImplementedException();
        }

        public void SetProvider(RPCProvider _provider)
        {
            provider = _provider;

        }
        public void Provider()
        {
            throw new System.NotImplementedException();
        }

        

        

        



    }
}