using System.Collections;
using System.Numerics;
using NUnit.Framework;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using UnityEngine;
using UnityEngine.TestTools;

namespace Sequence.Relayer.Tests
{
    public class WaaSTransactionQueuerTests : MonoBehaviour
    {
        private MonoBehaviour _testMonobehaviour;
        private SequenceWalletTransactionQueuer _queuer;
        private Address _fromAddress = new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249");
        private Address _toAddress = new Address("0x1099542D7dFaF6757527146C0aB9E70A967f71C0");
        private Address _contract = new Address("0x606e6d28e9150D8A3C070AEfB751a2D0C5DB19fa");
        
        [UnitySetUp]
        public IEnumerator Setup()
        {
            GameObject testObject = Instantiate(new GameObject("TestObject"));
            _testMonobehaviour = testObject.AddComponent<TestClass>();
            _queuer = _testMonobehaviour.gameObject.AddComponent<SequenceWalletTransactionQueuer>();
            _queuer.Setup(new MockWaaSWallet(), Chain.None);
            yield return null;
        }

        [UnityTest]
        public IEnumerator QueuedTokenTransactionsAreUpdatedAndAssembledAppropriately()
        {
            _queuer.Enqueue(new QueuedTokenTransaction(
                QueuedTokenTransaction.TransactionType.BURN,
                QueuedTokenTransaction.TokenType.ERC20,
                _contract,
                "1",
                BigInteger.One, 
                _fromAddress));
            _queuer.Enqueue(new QueuedTokenTransaction(
                QueuedTokenTransaction.TransactionType.BURN,
                QueuedTokenTransaction.TokenType.ERC20,
                _contract,
                "1",
                BigInteger.Parse("41"),
                _fromAddress));
            _queuer.Enqueue(new QueuedTokenTransaction(
                QueuedTokenTransaction.TransactionType.TRANSFER,
                QueuedTokenTransaction.TokenType.ERC721,
                _contract,
                "22",
                BigInteger.One, 
                _fromAddress,
                _toAddress));
            _queuer.Enqueue(new QueuedTokenTransaction(
                QueuedTokenTransaction.TransactionType.TRANSFER,
                QueuedTokenTransaction.TokenType.ERC721,
                _contract,
                "22",
                BigInteger.One, 
                _fromAddress,
                _fromAddress));
            _queuer.Enqueue(new QueuedTokenTransaction(
                QueuedTokenTransaction.TransactionType.MINT,
                QueuedTokenTransaction.TokenType.ERC1155,
                _contract,
                 "488",
                BigInteger.Parse("37"),
                _fromAddress,
                _toAddress));
            _queuer.Enqueue(new QueuedTokenTransaction(
                QueuedTokenTransaction.TransactionType.MINT,
                QueuedTokenTransaction.TokenType.ERC1155,
                _contract,
                "488",
                BigInteger.Parse("3"),
                _fromAddress,
                _toAddress));

            string asString = _queuer.ToString();
            Assert.IsTrue(asString.StartsWith("4 Queued Transactions: "));

            Transaction[] transactions = _queuer.BuildTransactions();
            int length = transactions.Length;
            Assert.AreEqual(4, length);

            ERC20 erc20 = new ERC20(_contract);
            ERC721 erc721 = new ERC721(_contract);
            ERC1155 erc1155 = new ERC1155(_contract);

            Transaction[] expected = new Transaction[]
            {
                new RawTransaction(erc20.Burn(BigInteger.Parse("42"))),
                new RawTransaction(erc721.SafeTransferFrom(_fromAddress, _toAddress, BigInteger.Parse("22"))),
                new RawTransaction(erc721.SafeTransferFrom(_fromAddress, _fromAddress, BigInteger.Parse("22"))),
                new RawTransaction(erc1155.Mint(_toAddress, BigInteger.Parse("488"), BigInteger.Parse("40")))
            };

            for (int i = 0; i < length; i++)
            {
                RawTransaction expectedTransaction = expected[i] as RawTransaction;
                RawTransaction actual = transactions[i] as RawTransaction;
                Assert.AreEqual(expectedTransaction.to, actual.to);
                Assert.AreEqual(expectedTransaction.type, actual.type);
                Assert.AreEqual(expectedTransaction.value, actual.value);
                Assert.AreEqual(expectedTransaction.data, actual.data);
            }
            yield return null;
        }
    }
}