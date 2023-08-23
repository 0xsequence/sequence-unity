using System.Collections.Generic;
using UnityEngine;

namespace Sequence.Demo
{
    public class ObjectPool
    {
        private GameObject _prefab;
        private int _objectCount;
        private GameObject[] _objects;
        private Queue<GameObject> _available;

        private ObjectPool(GameObject toInstantiate, int count)
        {
            this._prefab = toInstantiate;
            this._objectCount = count;
            this._objects = new GameObject[count];
            _available = new Queue<GameObject>();
        }

        public static ObjectPool ActivateObjectPool(GameObject toInstantiate, int count, Transform parent = null)
        {
            ObjectPool pool = new ObjectPool(toInstantiate, count);
            for (int i = 0; i < count; i++)
            {
                GameObject newObject;
                if (parent != null)
                {
                    
                    newObject = GameObject.Instantiate(toInstantiate, parent);
                }
                else
                {
                    newObject = GameObject.Instantiate(toInstantiate);
                }
                pool._objects[i] = newObject;
                pool._available.Enqueue(newObject);
                newObject.SetActive(false);
            }
            return pool;
        }

        public Transform GetNextAvailable()
        {
            GameObject next = _available.Dequeue();
            next.SetActive(true);
            return next.transform;
        }

        public void DeactivateObject(GameObject item)
        {
            item.SetActive(false);
            _available.Enqueue(item);
        }
    }
}