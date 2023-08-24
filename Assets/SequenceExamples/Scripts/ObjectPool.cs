using System.Collections.Generic;
using UnityEngine;

namespace Sequence.Demo
{
    public class ObjectPool
    {
        private GameObject _prefab;
        private int _objectCount;
        private Queue<GameObject> _available;
        private bool _canGrow;
        private Transform _parent;

        private ObjectPool(GameObject toInstantiate, int count, bool canGrow = true, Transform parent = null)
        {
            this._prefab = toInstantiate;
            this._objectCount = count;
            this._canGrow = canGrow;
            this._parent = parent;
            _available = new Queue<GameObject>();
        }

        public static ObjectPool ActivateObjectPool(GameObject toInstantiate, int count, bool canGrow = true, Transform parent = null)
        {
            ObjectPool pool = new ObjectPool(toInstantiate, count);
            for (int i = 0; i < count; i++)
            {
                pool.InstantiateObject();
            }
            return pool;
        }

        public Transform GetNextAvailable()
        {
            if (_available.Count == 0)
            {
                if (_canGrow)
                {
                    InstantiateObject();
                }
                else
                {
                    return null;
                }
            }
            GameObject next = _available.Dequeue();
            next.SetActive(true);
            return next.transform;
        }

        private void InstantiateObject()
        {
            GameObject newObject;
            if (_parent != null)
            {
                newObject = GameObject.Instantiate(_prefab, _parent);
            }
            else
            {
                newObject = GameObject.Instantiate(_prefab);
            }
            _available.Enqueue(newObject);
            newObject.SetActive(false);
        }

        public void DeactivateObject(GameObject item)
        {
            item.SetActive(false);
            _available.Enqueue(item);
        }
    }
}