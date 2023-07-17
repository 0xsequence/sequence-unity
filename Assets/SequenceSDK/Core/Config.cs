using System;
using Sequence.Core.Wallet;
using Sequence.Core.Signature;
using System.Collections.Generic;

namespace Sequence.Core {
    public static class Config
    {

        public static string AddressFromWalletConfig(IWalletConfig walletConfig, WalletContext context)
        {
            
            ImageHash imageHash = ImageHashOfWalletConfig(walletConfig);
            return AddressFromImageHash(imageHash.Hash, context);
            
        }

        public static string AddressFromImageHash(string imageHash, WalletContext context)
        {

            throw new NotImplementedException();


        }

        public static ImageHash ImageHashOfWalletConfig(IWalletConfig walletConfig)
        {
            throw new NotImplementedException();
        }

        private static byte[] ImageHashOfWalletConfigBytes(IWalletConfig walletConfig)
        {
            throw new NotImplementedException();
        }



        public static bool V1SortWalletConfig(IWalletConfig walletConfig)
        {
            WalletConfigSigners signers = GetSigners(walletConfig);
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

        private static WalletConfigSigners GetSigners(IWalletConfig walletConfig)
        {
            Dictionary<Address, UInt16> signers = walletConfig.Signers();
            WalletConfigSigners walletConfigSigners = new WalletConfigSigners();
            foreach (KeyValuePair<Address, UInt16> signer in signers)
            {
                WalletConfigSigner walletConfigSigner = new WalletConfigSigner();
                walletConfigSigner.Address = signer.Key;
                walletConfigSigner.Weight = (byte)(signer.Value);
                walletConfigSigners.Add(walletConfigSigner);
            }
            return walletConfigSigners;
        }

        public static bool IsWalletConfigUsable(IWalletConfig walletConfig)
        {
            throw new NotImplementedException();
        }

        public static bool IsWalletConfigEqual(IWalletConfig walletConfigA, IWalletConfig walletConfigB)
        {
            throw new NotImplementedException();
        }

    }
}