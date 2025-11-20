using System.Numerics;
using Sequence.Contracts;
using Sequence.Transactions;
using Sequence.Utils;

namespace Sequence.EcosystemWallet
{
    public static class TransactionConverter
    {
        public static ITransaction[] ToEcosystemWalletTransactions(this EmbeddedWallet.RawTransaction[] transactions)
        {
            var newTransactions = new ITransaction[transactions.Length];
            for (var i = 0; i < transactions.Length; i++)
                newTransactions[i] = transactions[i].ToEcosystemWalletTransaction();
            
            return newTransactions;
        }
        
        public static ITransaction ToEcosystemWalletTransaction(this EmbeddedWallet.RawTransaction transaction)
        {
            return new RawTransaction(
                new Address(transaction.to), 
                BigInteger.Parse(transaction.value), 
                transaction.data.HexStringToByteArray());
        }

        public static ITransaction ToEcosystemWalletTransaction(this CallContractFunction contract)
        {
            return new RawTransaction(
                new Address(contract.Address), 0, 
                contract.CallData.HexStringToByteArray());
        }
        
        public static ITransaction ToEcosystemWalletTransaction(this EthTransaction transaction)
        {
            return new RawTransaction(
                new Address(transaction.To), 
                transaction.Value, 
                transaction.Data.HexStringToByteArray());
        }
    }
}
