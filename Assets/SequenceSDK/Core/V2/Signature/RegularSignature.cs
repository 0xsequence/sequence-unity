using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Numerics;
using Sequence.Core.Signature;
using Sequence.Core.Wallet;
using Sequence.Core;
using System.Threading.Tasks;

namespace Sequence.Core.V2.Signature
{
    public class RegularSignature : ISignature
    {
        public bool IsRegular { get; set; }
        private UInt16 threshold;
        private UInt32 checkpoint;
        public ISignatureTree Tree { get; set; }

        public UInt32 Checkpoint()
        {
            return checkpoint;
        }

        public byte[] Data()
        {
            throw new NotImplementedException();
        }

        public ISignature Join(Subdigest subdigest, ISignature otherSignature)
        {
            throw new NotImplementedException();
        }

        public async Task<(IWalletConfig, BigInteger)> Recover(WalletContext context, Digest digest, Address wallet, BigInteger chainId, RPCProvider provider, List<SignerSignatures> signerSignatures)
        {
            if (chainId == null)
            {
                if (provider == null)
                {
                    throw new ArgumentException("Provider is required if chain Id is not specified");
                }

                chainId = await provider.ChainID();
            }

            return RecoverSubdigest(context, digest.Subdigest(wallet, chainId), provider, signerSignatures);
        }

        public (IWalletConfig, BigInteger) RecoverSubdigest(WalletContext context, Subdigest subdigest, RPCProvider provider, List<SignerSignatures> signerSignatures)
        {
            throw new NotImplementedException();
        }

        public ISignature Reduce(Subdigest subdigest)
        {
            throw new NotImplementedException();
        }

        public UInt16 Threshold()
        {
            return threshold;
        }
    }
}