using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sequence.Wallet;
namespace Sequence
{

    public class WalletContext
    {
        public string FactoryAddress { get; set; }
        public string MainModuleAddress { get; set; }
        public string MainModuleUpgradableAddress { get; set; }
        public string GuestModuleAddress { get; set; }

        public string UtilsAddress { get; set; }

    }

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