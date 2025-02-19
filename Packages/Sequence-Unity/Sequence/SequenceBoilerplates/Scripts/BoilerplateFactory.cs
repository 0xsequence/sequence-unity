using System;
using System.Collections.Generic;
using Sequence.Boilerplates.SignMessage;
using Sequence.Demo;
using Sequence.EmbeddedWallet;
using SequenceSDK.Samples;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sequence.Boilerplates
{
    public static class BoilerplateFactory
    {
        private static Dictionary<Type, GameObject> _objects = new();
        
        public static SequenceLoginWindow OpenSequenceLoginWindow(Transform parent)
        {
            return GetOrSpawnBoilerplate<SequenceLoginWindow>("Login/SequenceLoginWindow", parent, 
                b => b.Show());
        }

        public static SequencePlayerProfile OpenSequencePlayerProfile(Transform parent, IWallet wallet, Chain chain, Action onClose = null)
        {
            return GetOrSpawnBoilerplate<SequencePlayerProfile>("PlayerProfile/SequencePlayerProfile", parent, 
                b => b.Show(wallet, chain, onClose));
        }

        public static SequenceDailyRewards OpenSequenceDailyRewards(Transform parent, IWallet wallet, Chain chain, string apiUrl, Action onClose = null)
        {
            return GetOrSpawnBoilerplate<SequenceDailyRewards>("DailyRewards/SequenceDailyRewards", parent, 
                b => b.Show(wallet, chain, apiUrl, onClose));
        }
        
        public static SequenceInventory OpenSequenceInventory(Transform parent, IWallet wallet, Chain chain, string contractAddress, Action onClose = null)
        {
            return GetOrSpawnBoilerplate<SequenceInventory>("Inventory/SequenceInventory", parent, 
                b => b.Show(wallet, chain, contractAddress, onClose));
        }
        
        public static SequenceInGameShop OpenSequenceInGameShop(Transform parent, IWallet wallet, Chain chain, 
            string tokenContractAddress, string saleContractAddress, int[] itemsForSale, Action onClose = null)
        {
            return GetOrSpawnBoilerplate<SequenceInGameShop>("InGameShop/SequenceInGameShop", parent, 
                b => b.Show(wallet, chain, tokenContractAddress, saleContractAddress, itemsForSale, onClose));
        }
        
        public static SequenceSignMessage OpenSequenceSignMessage(Transform parent, IWallet wallet, Chain chain, Action onClose = null)
        {
            return GetOrSpawnBoilerplate<SequenceSignMessage>("SignMessage/SequenceSignMessage", parent, 
                b => b.Show(wallet, chain, onClose));
        }
        
        private static T GetOrSpawnBoilerplate<T>(string path, Transform parent, Action<T> show) where T : MonoBehaviour
        {
            var type = typeof(T);
            if (_objects.ContainsKey(type))
            {
                var cachedBoilerplate = _objects[type].GetComponent<T>();
                show.Invoke(cachedBoilerplate);
                return cachedBoilerplate;
            }
            
            var prefab = ((GameObject)Resources.Load($"Prefabs/{path}")).GetComponent<T>();
            if (prefab == null)
                throw new Exception($"Prefab at {path} not found in Resources folder");
            
            var boilerplate = Object.Instantiate(prefab, parent);
            _objects.Add(type, boilerplate.gameObject);
            show.Invoke(boilerplate);
            return boilerplate;
        }
    }
}