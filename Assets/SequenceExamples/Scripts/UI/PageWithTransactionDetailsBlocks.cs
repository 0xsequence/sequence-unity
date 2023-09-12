using System;
using System.Collections.Generic;
using Sequence.Demo.ScriptableObjects;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class PageWithTransactionDetailsBlocks : UIPage
    {
        [SerializeField] private GameObject _transactionDetailsBlockPlaceholderPrefab;
        [SerializeField] private int _numberOfTransactionDetailsBlockPlaceholdersToInstantiate = 1;
        [SerializeField] private int _numberOfTransactionDetailsToFetchAtOnce = 1;
        [SerializeField] private VerticalLayoutGroup _transactionDetailsBlockLayoutGroup;

        protected NetworkIcons _networkIcons;
        protected ICurrencyRepository _currencyRepository;
        private ObjectPool _transactionPool;
        private ITransactionDetailsFetcher _transactionDetailsFetcher;
        private List<TransactionDetails> _transactionDetails = new List<TransactionDetails>();
        private List<TransactionDetailsBlock> _transactionDetailsBlocks = new List<TransactionDetailsBlock>();
        private RectTransform _transactionDetailsBlockLayoutGroupRectTransform;
        
        protected override void Awake()
        {
            base.Awake();
            _transactionDetailsBlockLayoutGroupRectTransform =
                _transactionDetailsBlockLayoutGroup.GetComponent<RectTransform>();
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            NetworkIcons networkIcons = args.GetObjectOfTypeIfExists<NetworkIcons>();
            if (networkIcons == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(NetworkIcons)} as an argument");
            }
            ICurrencyRepository currencyRepository = args.GetObjectOfTypeIfExists<ICurrencyRepository>();
            if (currencyRepository == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(ICurrencyRepository)} as an argument");
            }
            ITransactionDetailsFetcher transactionDetailsFetcher =
                args.GetObjectOfTypeIfExists<ITransactionDetailsFetcher>();
            if (transactionDetailsFetcher == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(ITransactionDetailsFetcher)} as an argument");
            }
            
            _networkIcons = networkIcons;
            _currencyRepository = currencyRepository;

            _transactionPool = ObjectPool.ActivateObjectPool(_transactionDetailsBlockPlaceholderPrefab,
                _numberOfTransactionDetailsBlockPlaceholdersToInstantiate);
            
            _transactionDetailsFetcher = transactionDetailsFetcher;
            _transactionDetailsFetcher.OnTransactionDetailsFetchSuccess += HandleTransactionDetailsFetchSuccess;
            _transactionDetailsFetcher.Refresh();
            _transactionDetailsFetcher.FetchTransactionsFromContract(_currencyRepository.GetContractAddress(), _numberOfTransactionDetailsToFetchAtOnce);

            UpdateScrollViewSize();
        }

        public override void Close()
        {
            base.Close();
            _transactionDetailsFetcher.OnTransactionDetailsFetchSuccess -= HandleTransactionDetailsFetchSuccess;
            _transactionDetailsFetcher = null;
            _transactionPool.Cleanup();
            _transactionDetails = new List<TransactionDetails>();
            _transactionDetailsBlocks = new List<TransactionDetailsBlock>();
        }
        
        private void HandleTransactionDetailsFetchSuccess(FetchTransactionDetailsResult result)
        {
            TransactionDetails[] elements = result.Content;
            int count = elements.Length;
            for (int i = 0; i < count; i++)
            {
                _transactionDetails.Add(elements[i]);
                CreateTransactionDetailsBlock();
                UpdateScrollViewSize();
                SortTransactionDetails();
                PopulateTransactionDetailsBlocks();
            }

            if (result.MoreToFetch)
            {
                _transactionDetailsFetcher.FetchTransactionsFromContract(_currencyRepository.GetContractAddress(), _numberOfTransactionDetailsToFetchAtOnce);
            }
        }

        private void CreateTransactionDetailsBlock()
        {
            Transform transactionContainer = _transactionPool.GetNextAvailable();
            if (transactionContainer == null)
            {
                throw new SystemException(
                    $"{nameof(transactionContainer)} should not be null. {nameof(_transactionPool)} should expand.");
            }

            transactionContainer.SetParent(_transactionDetailsBlockLayoutGroupRectTransform);
            transactionContainer.localScale = new Vector3(1, 1, 1);
            TransactionDetailsBlock uiElement = transactionContainer.GetComponent<TransactionDetailsBlock>();
            _transactionDetailsBlocks.Add(uiElement);
        }

        protected virtual void UpdateScrollViewSize()
        {
            float contentHeight = _transactionDetailsBlockLayoutGroup.preferredHeight;
            _transactionDetailsBlockLayoutGroupRectTransform.sizeDelta =
                new Vector2(_transactionDetailsBlockLayoutGroupRectTransform.sizeDelta.x, contentHeight);
        }

        private void SortTransactionDetails()
        {
            int count = _transactionDetails.Count;

            for (int i = 0; i < count - 1; i++)
            {
                for (int j = 0; j < count - i - 1; j++)
                {
                    DateTime date1, date2;

                    if (DateTime.TryParse(_transactionDetails[j].Date, out date1) &&
                        DateTime.TryParse(_transactionDetails[j + 1].Date, out date2))
                    {
                        if (date1 < date2)
                        {
                            TransactionDetails temp = _transactionDetails[j];
                            _transactionDetails[j] = _transactionDetails[j + 1];
                            _transactionDetails[j + 1] = temp;
                        }
                    }
                    else
                    {
                        throw new SystemException($"{nameof(_transactionDetails)} contains an invalid date");
                    }
                }
            }
        }

        private void PopulateTransactionDetailsBlocks()
        {
            int length = _transactionDetailsBlocks.Count;
            for (int i = 0; i < length; i++)
            {
                _transactionDetailsBlocks[i].Assemble(_transactionDetails[i], _networkIcons);
            }
        }
    }
}