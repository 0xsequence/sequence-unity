using System;

namespace Sequence.Core.Wallet {
    public class WalletConfig
    {
        public int Threshold { get; set; }
        public WalletConfigSigners Signers { get; set; }

        public static string AddressFromWalletConfig(WalletConfig walletConfig, WalletContext context)
        {
            
            ImageHash imageHash = ImageHashOfWalletConfig(walletConfig);
            return AddressFromImageHash(imageHash.Hash, context);
            
        }

        public static string AddressFromImageHash(string imageHash, WalletContext context)
        {

            throw new NotImplementedException();


        }

        public static ImageHash ImageHashOfWalletConfig(WalletConfig walletConfig)
        {
            throw new NotImplementedException();
        }

        private static byte[] ImageHashOfWalletConfigBytes(WalletConfig walletConfig)
        {
            throw new NotImplementedException();
        }



        public static bool SortWalletConfig(WalletConfig walletConfig)
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

        public static bool IsWalletConfigUsable(WalletConfig walletConfig)
        {
            throw new NotImplementedException();
        }

        public static bool IsWalletConfigEqual(WalletConfig walletConfigA, WalletConfig walletConfigB)
        {
            throw new NotImplementedException();
        }

    }
}