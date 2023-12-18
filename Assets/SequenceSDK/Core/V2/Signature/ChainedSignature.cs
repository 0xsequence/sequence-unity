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
    public class ChainedSignature : ISignature
    {
        public uint Checkpoint()
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

        public Task<(IWalletConfig, BigInteger)> Recover(WalletContext context, Digest digest, Address wallet, BigInteger chainId, RPCProvider provider, params SignerSignatures[] signerSignatures)
        {
            throw new NotImplementedException();
        }

        public (IWalletConfig, BigInteger) RecoverSubdigest(WalletContext context, Subdigest subdigest, RPCProvider provider, params SignerSignatures[] signerSignatures)
        {
            throw new NotImplementedException();
        }

        public ISignature Reduce(Subdigest subdigest)
        {
            throw new NotImplementedException();
        }

        public ushort Threshold()
        {
            throw new NotImplementedException();
        }
    }
}