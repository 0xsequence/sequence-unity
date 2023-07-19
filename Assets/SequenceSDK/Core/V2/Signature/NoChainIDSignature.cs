using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Numerics;
using Sequence.Core.Signature;
using Sequence.Core.Wallet;
using Sequence.Extensions;
using System.Threading.Tasks;
using Sequence.Core.V2.Wallet;

namespace Sequence.Core.V2.Signature
{
    public class NoChainIDSignature : ISignature
    {
        private UInt16 threshold;
        private UInt32 checkpoint;
        public ISignatureTree Tree { get; set; }

        public NoChainIDSignature(UInt16 threshold, UInt32 checkpoint, ISignatureTree tree)
        {
            this.threshold = threshold;
            this.checkpoint = checkpoint;
            this.Tree = tree;
        }

        public uint Checkpoint()
        {
            return this.checkpoint;
        }

        public byte[] Data()
        {
            byte[] data = ByteArrayExtensions.ConcatenateByteArrays(
                SignatureType.NoChainID.ToByteArray(),
                this.threshold.ToByteArray(),
                this.checkpoint.ToByteArray(),
                this.Tree.Data());
            return data;
        }

        public ISignature Join(Subdigest subdigest, ISignature otherSignature)
        {
            NoChainIDSignature other = SignatureJoinParameterValidator.ValidateParameters<NoChainIDSignature>(this, otherSignature);

            ISignatureTree tree = this.Tree.Join(other.Tree);

            return new NoChainIDSignature(this.threshold, this.checkpoint, tree);
        }

        public async Task<(IWalletConfig, BigInteger)> Recover(WalletContext context, Digest digest, Address wallet, BigInteger chainId, RPCProvider provider, params SignerSignatures[] signerSignatures)
        {
            return RecoverSubdigest(context, digest.Subdigest(wallet), provider, signerSignatures);
        }

        public (IWalletConfig, BigInteger) RecoverSubdigest(WalletContext context, Subdigest subdigest, RPCProvider provider, params SignerSignatures[] signerSignatures)
        {
            if (signerSignatures == null)
            {
                signerSignatures = new SignerSignatures[0];
            }

            (IWalletConfigTree tree, BigInteger weight) = this.Tree.Recover(context, subdigest, provider, signerSignatures[0]);

            return (new WalletConfig(this.threshold, this.checkpoint, tree), weight);
        }

        public ISignature Reduce(Subdigest subdigest)
        {
            return new NoChainIDSignature(this.threshold, this.checkpoint, this.Tree.Reduce());
        }

        public ushort Threshold()
        {
            return this.threshold;
        }
    }
}