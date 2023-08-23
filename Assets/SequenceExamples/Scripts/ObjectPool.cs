using System.Collections.Generic;
using UnityEngine;

namespace Sequence.Demo
{
    public class ObjectPool : MonoBehaviour
    {
        private GameObject _prefab;
        private int _objectCount;
        private GameObject[] _objects;
        private Queue<GameObject> _available;

        public static ObjectPool ActivateObjectPool(GameObject caller, GameObject toInstantiate, int count, Transform parent = null)
        {
            ObjectPool pool = caller.AddComponent<ObjectPool>();
            pool._prefab = toInstantiate;
            pool._objectCount = count;
            pool._objects = new GameObject[count];
            pool._available = new Queue<GameObject>();
            for (int i = 0; i < count; i++)
            {
                GameObject newObject;
                if (parent != null)
                {
                    
                    newObject = Instantiate(toInstantiate, parent);
                }
                else
                {
                    newObject = Instantiate(toInstantiate, new Vector3(0, -1000000, 1), Quaternion.identity);
                }
                pool._objects[i] = newObject;
                pool._available.Enqueue(newObject);
                newObject.SetActive(false);
            }
            return pool;
        }

        public Transform GetNextAvailable()
        {
            if (_available.Count == 0)
            {
                return null;
            }
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