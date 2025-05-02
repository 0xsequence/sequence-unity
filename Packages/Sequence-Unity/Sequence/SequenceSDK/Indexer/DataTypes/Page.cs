using System;

namespace Sequence
{
    [System.Serializable]
    public class Page
    {
        [Obsolete("Page number is now deprecated. Instead, simply provide the page you are given to fetch the next page.")]
        public int page;
        public string column;
        public object before;
        public object after;
        public SortBy[] sort;
        public int pageSize = 40;
        public bool more;
    }
}