using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sequence.Boilerplates
{
    [RequireComponent(typeof(Button))]
    public class FeatureSelectionButton : MonoBehaviour
    {
        public UnityAction ExecuteClick => () => GetComponent<Button>().onClick?.Invoke();
        
        [field: SerializeField] public string Key { get; private set; }

        public void EnableIfExists(string[] allFeatures)
        {
            gameObject.SetActive(allFeatures.Length == 0 || Array.Exists(allFeatures, f => f == Key));
        }
    }
}