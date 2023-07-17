using System;
using System.Collections.Generic;

namespace Sequence.Core.Wallet {
    
    public class WalletConfigSigner
    {
        public byte Weight { get; set; }
        public string Address { get; set; }
    }

    public class WalletConfigSigners : List<WalletConfigSigner>
    {
        public int Len() => Count;

        public bool Less(int i, int j)
        {
            //TODO:
            throw new NotImplementedException();
        }

        public void Swap(int i, int j)
        {
            WalletConfigSigner temp = this[i];
            this[i] = this[j];
            this[j] = temp;
        }

        public (byte, bool) GetWeightByAddress(string address)
        {
            foreach (WalletConfigSigner signer in this)
            {
                if (signer.Address == address)
                {
                    return (signer.Weight, true);
                }
            }
            return (0, false);
        }
    }
}