using System.Collections;
using Sequence;
using Sequence.Demo;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace SequenceExamples.Scripts.Tests
{
    public class TokenInfoPageTests
    {
        private MonoBehaviour _testMonoBehaviour;
        private TokenInfoPage _tokenInfoPage;

        public TokenInfoPageTests(MonoBehaviour testMonoBehaviour, TokenInfoPage tokenInfoPage)
        {
            _testMonoBehaviour = testMonoBehaviour;
            _tokenInfoPage = tokenInfoPage;
        }

        internal IEnumerator AssertTokenInfoPageIsAsExpected(Chain network, int randomNumberOfTransactionsToFetch, int delayInMillisecondsBetweenFetches)
        {
            Assert.IsNotNull(_tokenInfoPage);

            Transform networkBanner = _tokenInfoPage.transform.Find("NetworkBanner");
            Assert.IsNotNull(networkBanner);
            Transform networkIcon = networkBanner.transform.Find("NetworkIcon");
            Assert.IsNotNull(networkIcon);
            Image networkIconImage = networkIcon.GetComponent<Image>();
            Assert.IsNotNull(networkIconImage);
            Assert.AreEqual(_tokenInfoPage.GetNetworkIcon(network), networkIconImage.sprite);
            Transform networkName = networkBanner.transform.Find("NetworkName");
            Assert.IsNotNull(networkName);
            TextMeshProUGUI networkNameText = networkName.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(networkNameText);
            Assert.AreEqual(ChainNames.NameOf[network], networkNameText.text);

            Transform currencyValue = _tokenInfoPage.transform.Find("CurrencyValueText");
            Assert.IsNotNull(currencyValue);
            TextMeshProUGUI currencyValueText = currencyValue.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(currencyValueText);
            string currentCurrencyValue = currencyValueText.text;
            yield return new WaitForSecondsRealtime(_tokenInfoPage.TimeBetweenCurrencyValueRefreshesInSeconds);
            Assert.AreNotEqual(currentCurrencyValue, currencyValueText.text);

            GameObject transactionScrollView = GameObject.Find("TransactionsScrollView");
            Assert.IsNotNull(transactionScrollView);
            VerticalLayoutGroup transactionVerticalLayoutGroup =
                transactionScrollView.GetComponentInChildren<VerticalLayoutGroup>();
            Assert.IsNotNull(transactionVerticalLayoutGroup);
            TransactionDetailsBlocksUITests transactionDetailsBlocksUITests =
                new TransactionDetailsBlocksUITests(_testMonoBehaviour);
            yield return _testMonoBehaviour.StartCoroutine(transactionDetailsBlocksUITests.AssertTransactionDetailsBlocksAreAsExpected(transactionVerticalLayoutGroup, randomNumberOfTransactionsToFetch, delayInMillisecondsBetweenFetches));
        }
    }
}