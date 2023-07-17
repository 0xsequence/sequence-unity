using System.Numerics;
using System.Text;
using Sequence.ABI;
using Sequence.Wallet;

namespace Sequence.Core.Signature
{
    public class Subdigest
    {
        public string Hash { get; set; }
        // Digest is the preimage of the subdigest
        public Digest Digest { get; set; }
        // Wallet is the target wallet of the subdigest, *common.Address in go-sequence
        public string WalletAddress { get; set; }
        // ChainID is the target chain ID of the subdigest
        public BigInteger ChainID { get; set; }
        // EthSignPreimage is the preimage of the eth_sign subdigest
        public Subdigest EthSignPreimage { get; set; }

        // EthSignSubdigest derives the eth_sign subdigest of a subdigest.
        public Subdigest EthSignSubdigest()
        {
            return new Subdigest
            {
                //TODO : 
                Hash = SequenceCoder.ByteArrayToHexString(EthWallet.PrefixedMessage(Encoding.UTF8.GetBytes(this.Hash))),
                EthSignPreimage = this

            };
        }
    }
}