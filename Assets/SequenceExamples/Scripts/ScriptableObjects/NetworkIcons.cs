using System.Collections.Generic;
using UnityEngine;
using Sequence;

namespace Sequence.Demo.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Network Icons Mapping", menuName = "Sequence/Network Icons Mapping")]
    public class NetworkIcons : ScriptableObject
    {
        public Dictionary<Chain, Sprite> NetworkIconMapping = new Dictionary<Chain, Sprite>();
        public int Test;
    }
}