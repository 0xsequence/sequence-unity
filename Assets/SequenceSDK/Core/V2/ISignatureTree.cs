using System;
using System.Collections.Generic;
using System.Numerics;
using Sequence.Core.Provider;
using Sequence.Core.Signature;
using Sequence.Core.Wallet;


namespace Sequence.Core.V2
{
    public interface ISignatureTree
    {
        (IWalletConfigTree, BigInteger) Recover(WalletContext context,
                                            Subdigest subdigest,
                                            RPCProvider provider,
                                            List<SignerSignatures> signerSignatures);
    }
}
