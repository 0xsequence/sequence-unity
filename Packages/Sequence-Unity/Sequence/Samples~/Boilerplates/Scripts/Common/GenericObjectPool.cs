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

        private T[] _objectArr;
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

            var go = _objectQueue.Dequeue();
            go.gameObject.SetActive(true);

            var applyParent = parent == null ? _parent : parent;
            go.transform.SetParent(applyParent);
            
            if (_setAsLastSibling)
                go.transform.SetAsLastSibling();

            EnqueueObject(go);
            return go;
        }
        
        private void EnqueueObject(T go)
        {
            if (!_objectQueue.Contains(go))
                _objectQueue.Enqueue(go);
        }

        private void Populate()
        {
            _objectArr = new T[_amount];
            _objectQueue = new Queue<T>();
            
            for (int i = 0; i < _amount; i++)
            {
                var go = GameObject.Instantiate(_prefab, _parent);
                go.gameObject.SetActive(false);

                _objectArr[i] = go;
                EnqueueObject(go);
            }
        }
    }
}