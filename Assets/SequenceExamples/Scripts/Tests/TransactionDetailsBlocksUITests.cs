using System;
using System.Collections;
using Sequence.Demo;
using SequenceExamples.Scripts.Tests.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace SequenceExamples.Scripts.Tests
{
    public class TransactionDetailsBlocksUITests
    {
        private MonoBehaviour _testMonoBehaviour;

        public TransactionDetailsBlocksUITests(MonoBehaviour testMonoBehaviour)
        {
            _testMonoBehaviour = testMonoBehaviour;
        }

        public IEnumerator AssertTransactionDetailsBlocksAreAsExpected(int expected, int delayInMillisecondsBetweenFetches)
        {
            yield return new WaitForSeconds(expected * (float)delayInMillisecondsBetweenFetches / 1000); // Allow content to load
            
            GameObject transactionScrollView = GameObject.Find("TransactionsScrollView");
            Assert.IsNotNull(transactionScrollView);
            VerticalLayoutGroup transactionVerticalLayoutGroup =
                transactionScrollView.GetComponentInChildren<VerticalLayoutGroup>();
            Assert.IsNotNull(transactionVerticalLayoutGroup);
            int transactionDetailsBlocksCount = transactionVerticalLayoutGroup.transform.childCount;
            Assert.AreEqual(expected, transactionDetailsBlocksCount);

            TransactionDetailsBlock[] transactionDetailsBlocks =
                transactionVerticalLayoutGroup.GetComponentsInChildren<TransactionDetailsBlock>();
            Assert.IsNotNull(transactionDetailsBlocks);
            Assert.AreEqual(expected, transactionDetailsBlocks.Length);

            yield return _testMonoBehaviour.StartCoroutine(AssertBlocksAreAsExpected(transactionDetailsBlocks));
        }

        private IEnumerator AssertBlocksAreAsExpected(TransactionDetailsBlock[] blocks)
        {
            int count = blocks.Length;
            DateTime[] dates = new DateTime[count];
            string[] currencyValueStrings = new string[count];
            for (int i = 0; i < count; i++)
            {
                AssertValidReceivedSentText(blocks[i]);
                AssertCorrectNetworkIcon(blocks[i]);
                dates[i] = GetDate(blocks[i]);
                currencyValueStrings[i] = GetCurrencyValueString(blocks[i]);
            }
            Assert.IsTrue(IsSorted(dates));

            yield return new WaitForSeconds(blocks[0].TimeBetweenTokenValueRefreshesInSeconds);

            for (int i = 0; i < count; i++)
            {
                Assert.AreNotEqual(currencyValueStrings[i], GetCurrencyValueString(blocks[i]));
            }
        }

        private void AssertValidReceivedSentText(TransactionDetailsBlock block)
        {
            Transform sentReceivedTextTransform = block.transform.FindAmongDecendants("SentReceivedText");
            Assert.IsNotNull(sentReceivedTextTransform);
            TextMeshProUGUI sentReceivedText = sentReceivedTextTransform.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(sentReceivedText);
            Assert.IsTrue(sentReceivedText.text == "<b>↑</b>Sent" || sentReceivedText.text == "<b>↓</b>Received");
        }

        private void AssertCorrectNetworkIcon(TransactionDetailsBlock block)
        {
            Transform networkIconTransform = block.transform.FindAmongDecendants("NetworkIcon");
            Assert.IsNotNull(networkIconTransform);
            Image networkIcon = networkIconTransform.GetComponent<Image>();
            Assert.IsNotNull(networkIcon);
            Assert.AreEqual(block.GetNetworkIcon(), networkIcon.sprite);
        }

        private DateTime GetDate(TransactionDetailsBlock block)
        {
            Transform dateTextTransform = block.transform.FindAmongDecendants("DateText");
            Assert.IsNotNull(dateTextTransform);
            TextMeshProUGUI dateText = dateTextTransform.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(dateText);

            string dateString = dateText.text;
            DateTime date;
            Assert.IsTrue(DateTime.TryParse(dateString, out date));
            return date;
        }

        private string GetCurrencyValueString(TransactionDetailsBlock block)
        {
            Transform currencyValueTextTransform = block.transform.FindAmongDecendants("CurrencyValueText");
            Assert.IsNotNull(currencyValueTextTransform);
            TextMeshProUGUI currencyValueText = currencyValueTextTransform.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(currencyValueText);
            string currencyValueString = currencyValueText.text;
            return currencyValueString;
        }

        private bool IsSorted(DateTime[] dates)
        {
            int length = dates.Length;
            for (int i = 0; i < length - 1; i++)
            {
                if (dates[i] < dates[i + 1])
                {
                    return false;
                }
            }

            return true;
        }
    }
}