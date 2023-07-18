using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Numerics;
using Sequence.Core.Signature;
using Sequence.Core.Wallet;
using Sequence.Core;
using System.Threading.Tasks;
using Sequence.Core.V2.Wallet;
using Sequence.Extensions;

namespace Sequence.Core.V2.Signature
{
    public class RegularSignature : ISignature
    {
        public bool IsRegular { get; set; }
        private UInt16 threshold;
        private UInt32 checkpoint;
        public ISignatureTree Tree { get; set; }

        public RegularSignature(bool isRegular, UInt16 threshold, UInt32 checkpoint, ISignatureTree tree)
        {
            this.IsRegular = isRegular;
            this.threshold = threshold;
            this.checkpoint = checkpoint;
            this.Tree = tree;
        }

        public UInt32 Checkpoint()
        {
            return checkpoint;
        }

        public byte[] Data()
        {
            byte[] data = new byte[0];
            if (this.IsRegular)
            {
                data = this.IsRegular.ToByteArray();
            }
            data = ByteArrayExtensions.ConcatenateByteArrays(
                data,
                this.threshold.ToByteArray(),
                this.checkpoint.ToByteArray(),
                this.Tree.Data());
            return data;
        }

        public ISignature Join(Subdigest subdigest, ISignature otherSignature)
        {
            if (!(otherSignature is RegularSignature other))
            {
                throw new ArgumentException($"{nameof(otherSignature)} Expected RegularSignature, got {otherSignature.GetType()}");
            }

            if (this.threshold != other.threshold)
            {
                throw new ArgumentOutOfRangeException($"Threshold mismatch: {this.threshold} != {other.threshold}");
            }

            if (this.checkpoint != other.checkpoint)
            {
                throw new ArgumentOutOfRangeException($"Checkpoint mismatch: {this.checkpoint} != {other.checkpoint}");
            }

            ISignatureTree tree = this.Tree.Join(other.Tree);

            return new RegularSignature(this.IsRegular, this.threshold, this.checkpoint, tree);
        }

        public async Task<(IWalletConfig, BigInteger)> Recover(WalletContext context, Digest digest, Address wallet, BigInteger chainId, RPCProvider provider, params SignerSignatures[] signerSignatures)
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

        public (IWalletConfig, BigInteger) RecoverSubdigest(WalletContext context, Subdigest subdigest, RPCProvider provider, params SignerSignatures[] signerSignatures)
        {
            if (signerSignatures == null)
            {
                signerSignatures = new SignerSignatures[0];
            }

            (IWalletConfigTree tree, BigInteger weight) = this.Tree.Recover(context, subdigest, provider, signerSignatures);

            return (new WalletConfig(this.threshold, this.checkpoint, tree), weight);
        }

        public ISignature Reduce(Subdigest subdigest)
        {
            return new RegularSignature(this.IsRegular, this.threshold, this.checkpoint, this.Tree.Reduce());
        }

        public UInt16 Threshold()
        {
            return threshold;
        }
    }
}