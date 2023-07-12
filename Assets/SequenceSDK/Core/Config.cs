using System;
using Sequence.Core.Wallet;
using Sequence.Core.Signature;

namespace Sequence.Core {
    public class Config
    {
        public int Threshold { get; set; }
        public WalletConfigSigners Signers { get; set; }

        public static string AddressFromWalletConfig(Config walletConfig, WalletContext context)
        {
            
            ImageHash imageHash = ImageHashOfWalletConfig(walletConfig);
            return AddressFromImageHash(imageHash.Hash, context);
            
        }

        public static string AddressFromImageHash(string imageHash, WalletContext context)
        {

            throw new NotImplementedException();


        }

        public static ImageHash ImageHashOfWalletConfig(Config walletConfig)
        {
            throw new NotImplementedException();
        }

        private static byte[] ImageHashOfWalletConfigBytes(Config walletConfig)
        {
            throw new NotImplementedException();
        }



        public static bool SortWalletConfig(Config walletConfig)
        {
            WalletConfigSigners signers = walletConfig.Signers;
            signers.Sort(); // Sort the signers

            // Ensure no duplicates
            for (int i = 0; i < signers.Count - 1; i++)
            {
                if (signers[i].Address == signers[i + 1].Address)
                {
                    return false; // Signer duplicate detected in the wallet config
                }
            }

            return true;
        }

        public static bool IsWalletConfigUsable(Config walletConfig)
        {
            throw new NotImplementedException();
        }

        public static bool IsWalletConfigEqual(Config walletConfigA, Config walletConfigB)
        {
            throw new NotImplementedException();
        }

    }
}