using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sequence.WaaS
{
    public class SortOrder
    {
        public static string Descending = "DESC";
        public static string Ascending = "ASC";

        public string Value { get; private set; }

        public SortOrder(bool ascending)
        {
            if (ascending)
            {
                Value = Ascending;
            }
            else
            {
                Value = Descending;
            }
        }

        public static implicit operator string(SortOrder order)
        {
            return order.Value;
        }
    }
}
