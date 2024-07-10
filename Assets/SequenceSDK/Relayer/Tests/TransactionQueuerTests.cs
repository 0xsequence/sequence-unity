using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal.Commands;
using Sequence.WaaS;
using SequenceExamples.Scripts.Tests.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Sequence.Relayer.Tests
{
    public class TransactionQueuerTests : MonoBehaviour
    {
        private MonoBehaviour _testMonobehaviour;
        private TestTransactionQueuer _queuer;
        
        [UnitySetUp]
        public IEnumerator Setup()
        {
            GameObject testObject = Instantiate(new GameObject("TestObject"));
            _testMonobehaviour = testObject.AddComponent<TestClass>();
            _queuer = _testMonobehaviour.gameObject.AddComponent<TestTransactionQueuer>();
            _queuer.Setup(new MockWaaSWallet(), Chain.None);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator TestSubmitTransactionsClearsQueue()
        {
            _queuer.Enqueue(0);
            _queuer.Enqueue(1);
            _queuer.Enqueue(2);
            _queuer.Enqueue(3);
            _queuer.Enqueue(4);

            string asString = _queuer.ToString();
            Assert.AreEqual("5 Queued Transactions: 0 | 1 | 2 | 3 | 4", asString);

            var task = _queuer.SubmitTransactions(true);
            while (!task.IsCompleted)
            {
                yield return null;
            }
            bool result = task.Result;
            Assert.IsTrue(result);

            string newString = _queuer.ToString();
            Assert.AreNotEqual(asString, newString);
            Assert.AreEqual("0 Queued Transactions", newString);
        }

        [UnityTest]
        public IEnumerator TestTransactionQueuerAutoSubmitsTransactionsAsConfigured()
        {
            _queuer.ThresholdTimeBetweenTransactionsAddedBeforeSubmittedInSeconds = 5;
            _queuer.AutoSubmitTransactions = true;
            
            _queuer.Enqueue(0);
            _queuer.Enqueue(1);
            _queuer.Enqueue(2);
            _queuer.Enqueue(3);
            _queuer.Enqueue(4);
            string asString = _queuer.ToString();
            Assert.AreEqual("5 Queued Transactions: 0 | 1 | 2 | 3 | 4", asString);

            yield return new WaitForSecondsRealtime(5);

            string newString = _queuer.ToString();
            Assert.AreNotEqual(asString, newString);
            Assert.AreEqual("0 Queued Transactions", newString);
        }

        [UnityTest]
        public IEnumerator TestTransactionQueuerResetsTimerWhenTransactionAdded()
        {
            _queuer.ThresholdTimeBetweenTransactionsAddedBeforeSubmittedInSeconds = 5;
            _queuer.AutoSubmitTransactions = true;
            
            _queuer.Enqueue(0);
            _queuer.Enqueue(1);
            _queuer.Enqueue(2);
            _queuer.Enqueue(3);
            _queuer.Enqueue(4);
            string asString = _queuer.ToString();
            Assert.AreEqual("5 Queued Transactions: 0 | 1 | 2 | 3 | 4", asString);

            yield return new WaitForSecondsRealtime(3);
            
            _queuer.Enqueue(5);
            yield return new WaitForSecondsRealtime(2);

            string newString = _queuer.ToString();
            Assert.AreNotEqual(asString, newString);
            Assert.AreEqual("6 Queued Transactions: 0 | 1 | 2 | 3 | 4 | 5", newString);

            yield return new WaitForSecondsRealtime(3);
            
            string finalString = _queuer.ToString();
            Assert.AreNotEqual(newString, finalString);
            Assert.AreEqual("0 Queued Transactions", finalString);
        }

        [UnityTest]
        public IEnumerator TestTransactionQueuerBlocksTransactionsBeforeMinimumTimeElapsed()
        {
            _queuer.Enqueue(0);
            var task = _queuer.SubmitTransactions(true);
            while (!task.IsCompleted)
            {
                yield return null;
            }
            bool result = task.Result;
            Assert.IsTrue(result);
            
            _queuer.Enqueue(0);
            _queuer.Enqueue(1);
            _queuer.Enqueue(2);
            _queuer.Enqueue(3);
            _queuer.Enqueue(4);

            string asString = _queuer.ToString();
            Assert.AreEqual("5 Queued Transactions: 0 | 1 | 2 | 3 | 4", asString);

            task = _queuer.SubmitTransactions(true);
            while (!task.IsCompleted)
            {
                yield return null;
            }
            result = task.Result;
            Assert.IsTrue(result);

            string newString = _queuer.ToString();
            Assert.AreNotEqual(asString, newString);
            Assert.AreEqual("0 Queued Transactions", newString);
            
            _queuer.Enqueue(0);
            _queuer.Enqueue(1);
            _queuer.Enqueue(2);
            _queuer.Enqueue(3);
            _queuer.Enqueue(4);
            task = _queuer.SubmitTransactions();
            while (!task.IsCompleted)
            {
                yield return null;
            }
            result = task.Result;
            Assert.IsFalse(result);

            newString = _queuer.ToString();
            Assert.AreEqual(asString, newString);
        }
    }

    public class TestTransactionQueuer : TransactionQueuer<int, bool>
    {
        protected override async Task<bool> DoSubmitTransactions(bool waitForReceipt = true)
        {
            return true;
        }
    }

    public class TestClass : MonoBehaviour
    {
        // Used to attach a monobehaviour to our test object. Unity requires a monobehaviour to attach a coroutine to - that way it can cancel the coroutine if the monobehaviour
        // gets destroyed. The test object will not be destroyed, allowing our tests to run to completion
    }
}