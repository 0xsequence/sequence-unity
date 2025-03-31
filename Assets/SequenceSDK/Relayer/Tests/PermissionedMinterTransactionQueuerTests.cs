using System.Collections;
using NUnit.Framework;
using Sequence.EmbeddedWallet;
using UnityEngine;
using UnityEngine.TestTools;

namespace Sequence.Relayer.Tests
{
    public class PermissionedMinterTransactionQueuerTests : MonoBehaviour
    {
        private PermissionedMinterTransactionQueuer _queuer;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            GameObject testObject = Instantiate(new GameObject("TestObject"));
            _queuer = testObject.AddComponent<PermissionedMinterTransactionQueuer>();
            _queuer.Setup(new MockWaaSWallet(), Chain.None, "", "0xc683a014955b75f5ecf991d4502427c8fa1aa249");
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestTransactionsAreCombinedCorrectly()
        {
            _queuer.Enqueue(new PermissionedMintTransaction("1", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("1", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("2", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("1", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("20", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("1", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("20", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("1", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("3", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("3", 5));

            string result = _queuer.ToString();
            Assert.AreEqual("4 Queued Transactions: (Mint 25 of Token Id 1, Sequence.Relayer.PermissionedMinter) | (Mint 5 of Token Id 2, Sequence.Relayer.PermissionedMinter) | (Mint 10 of Token Id 20, Sequence.Relayer.PermissionedMinter) | (Mint 10 of Token Id 3, Sequence.Relayer.PermissionedMinter)", result);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestTransactionSubmission()
        {
            _queuer.InjectNewMinter(new MockMinterThatCantMintOddAmounts());
            
            _queuer.Enqueue(new PermissionedMintTransaction("1", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("1", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("2", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("1", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("20", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("1", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("20", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("1", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("3", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("3", 5));

            var task = _queuer.SubmitTransactions(true);
            while (!task.IsCompleted)
            {
                yield return null;
            }
            PermissionedMinterQueueSubmissionResult result = task.Result;
            
            Assert.IsFalse(result.Success);
            Assert.AreEqual(2, result.TransactionHashes.Length);
        }

        [UnityTest]
        public IEnumerator TestTransactionSubmission_allPass()
        {
            _queuer.InjectNewMinter(new MockMinterThatCantMintOddAmounts());
            
            _queuer.Enqueue(new PermissionedMintTransaction("1", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("1", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("1", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("20", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("1", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("20", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("3", 5));
            _queuer.Enqueue(new PermissionedMintTransaction("3", 5));

            var task = _queuer.SubmitTransactions(true);
            while (!task.IsCompleted)
            {
                yield return null;
            }
            PermissionedMinterQueueSubmissionResult result = task.Result;
            
            Assert.IsTrue(result.Success);
            Assert.AreEqual(3, result.TransactionHashes.Length);
        }
    }
}