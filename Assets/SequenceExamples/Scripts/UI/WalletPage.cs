using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Sequence.Demo
{
    public class WalletPage : UIPage
    {
        [SerializeField] private int numberOfNftPlaceholdersToInstantiate = 10;
        [SerializeField] private GameObject NftPlaceHolderPrefab;
        private ObjectPool _pool;
        private INftContentFetcher _nftFetcher;
        private Texture2D[] _nftContent;
        
        protected override void Awake()
        {
            base.Awake();
            _pool = ObjectPool.ActivateObjectPool(NftPlaceHolderPrefab, numberOfNftPlaceholdersToInstantiate);
        }

        public override void Open(params object[] args)
        {
            base.Open();

            if (_nftFetcher == null)
            {
                throw new SystemException($"{nameof(_nftFetcher)} must not be null. Please call {nameof(SetupContentFetchers)} before opening");
            }

            StartCoroutine(FetchNftContent());
        }

        public override void Close()
        {
            base.Close();
            _nftFetcher = null;
        }

        public void SetupContentFetchers(INftContentFetcher nftContentFetcher)
        {
            _nftFetcher = nftContentFetcher;
        }

        private IEnumerator FetchNftContent()
        {
            Task<Texture2D[]> fetch = _nftFetcher.FetchContent();

            yield return new WaitUntil(() => fetch.IsCompleted);

            if (fetch.Exception != null)
            {
                Debug.LogError("Error fetching tokens: " + fetch.Exception);
            }

            Texture2D[] content = fetch.Result;
            if (content == null || content.Length == 0)
            {
                Debug.LogError("Error fetching tokens: none found");
                yield break;
            }

            _nftContent = content;
        }
    }
}