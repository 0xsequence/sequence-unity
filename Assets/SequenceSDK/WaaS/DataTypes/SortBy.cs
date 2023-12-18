using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sequence.WaaS
{
    [System.Serializable]
    public class SortBy
    {
        public string column { get; private set; }
        public SortOrder order { get; private set; }
    }
}
