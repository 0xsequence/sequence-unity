using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sequence.Boilerplates
{
    [Serializable]
    public class GenericObjectPool<T> where T : Component
    {
        public Transform Parent => _parent;

        [SerializeField] private int _amount = 1;
        [SerializeField] private bool _setAsLastSibling = true;
        [SerializeField] private Transform _parent;
        [SerializeField] private T _prefab;

        private Queue<T> _objectQueue;

        public void Cleanup()
        {
            if (Parent == null)
                return;
            
            foreach (Transform child in Parent)
                child.gameObject.SetActive(false);
        }

        public T GetObject(Transform parent = null)
        {
            if (_objectQueue == null)
                Populate();

            var go = Dequeue();
            go.gameObject.SetActive(true);

            var applyParent = parent == null ? _parent : parent;
            go.transform.SetParent(applyParent);
            
            if (_setAsLastSibling)
                go.transform.SetAsLastSibling();

            return go;
        }

        public void RecycleObject(T go)
        {
            go.gameObject.SetActive(false);
            EnqueueObject(go);
        }
        
        private void EnqueueObject(T go)
        {
            if (!_objectQueue.Contains(go))
                _objectQueue.Enqueue(go);
        }

        private T Dequeue()
        {
            if (_objectQueue == null)
            {
                Populate();
            }
            
            bool success = _objectQueue.TryDequeue(out T obj);
            if (!success)
            {
                _amount++;
                
                var go = GameObject.Instantiate(_prefab, _parent);
                go.gameObject.SetActive(false);
                obj = go;
            }

            return obj;
        }

        private void Populate()
        {
            _objectQueue = new Queue<T>();
            
            for (int i = 0; i < _amount; i++)
            {
                var go = GameObject.Instantiate(_prefab, _parent);
                go.gameObject.SetActive(false);

                EnqueueObject(go);
            }
        }
    }
}