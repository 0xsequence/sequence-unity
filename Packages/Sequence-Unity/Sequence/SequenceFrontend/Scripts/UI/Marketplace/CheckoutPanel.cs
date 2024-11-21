using System;
using System.Collections;
using Sequence.Marketplace;
using Sequence.Utils;

namespace Sequence.Demo
{
    public class CheckoutPanel : UIPanel
    {
        private Order[] _listings;

        public override void Open(params object[] args)
        {
            base.Open(args);
            _listings = args.GetObjectOfTypeIfExists<Order[]>();
            if (_listings == null || _listings.Length == 0)
            {
                throw new ArgumentException(
                    $"Invalid use. {GetType().Name} must be opened with a non-empty {typeof(Order[])} as an argument");
            }
        }
    }
}