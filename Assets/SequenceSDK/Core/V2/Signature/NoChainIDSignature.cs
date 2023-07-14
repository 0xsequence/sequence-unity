using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Numerics;
using Sequence.Core.Signature;
using Sequence.Core.Wallet;
using Sequence.Core;

namespace Sequence.Core.V2.Signature
{
    public class NoChainIDSignature : ISignature
    {
        public BigInteger Checkpoint()
        {
            throw new NotImplementedException();
        }

        public byte[] Data()
        {
            throw new NotImplementedException();
        }

        public ISignature Join(Subdigest subdigest, ISignature otherSignature)
        {
            throw new NotImplementedException();
        }

        public (IWalletConfig, BigInteger) Recover(WalletContext context, Digest digest, Address wallet, BigInteger chainId, RPCProvider provider, List<SignerSignatures> signerSignatures)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}