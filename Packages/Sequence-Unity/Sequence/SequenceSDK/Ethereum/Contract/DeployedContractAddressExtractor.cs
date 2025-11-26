namespace Sequence.EmbeddedWallet
{
    public static class DeployedeContractAddressExtractor
    {
        public static string ExtractFirstContractAddressExceptOwn(TransactionReceipt receipt, string ownContractAddress)
        {
            ownContractAddress = ownContractAddress?.ToLower();

            if (!string.IsNullOrEmpty(receipt.contractAddress))
            {
                var contractAddrLower = receipt.contractAddress.ToLower();
                if (contractAddrLower != ownContractAddress)
                {
                    return contractAddrLower;
                }
            }

            if (receipt.logs != null)
            {
                foreach (var log in receipt.logs)
                {
                    if (!string.IsNullOrEmpty(log.address))
                    {
                        var addrLower = log.address.ToLower();
                        if (addrLower != ownContractAddress)
                        {
                            return addrLower;
                        }
                    }
                }
            }

            return null;
        }
    }
}
