using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Sequence.WaaS
{
    [System.Serializable]
    public class Page
    {
        public uint? pageSize { get; private set; }
        public uint? page { get; private set; }
        public uint? totalRecords { get; private set; }
        [CanBeNull] public string column { get; private set; }
        [CanBeNull] public object before { get; private set; }
        [CanBeNull] public object after { get; private set; }
        [CanBeNull] public SortBy[] sort { get; private set; }
        
        public Page(uint? pageSize = null, uint? page = null, uint? totalRecords = null, string column = null, object before = null, object after = null, SortBy[] sort = null)
        {
            this.pageSize = pageSize;
            this.page = page;
            this.totalRecords = totalRecords;
            this.column = column;
            this.before = before;
            this.after = after;
            this.sort = sort;
        }
    }

}