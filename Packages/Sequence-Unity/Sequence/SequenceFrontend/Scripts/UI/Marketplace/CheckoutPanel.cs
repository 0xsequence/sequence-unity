using System;
using System.Collections;
using Sequence.Marketplace;
using Sequence.Utils;

namespace Sequence.Demo
{
    public class CheckoutPanel : UIPanel
    {
        public override void Open(params object[] args)
        {
            base.Open(args);
            Cart cart = args.GetObjectOfTypeIfExists<Cart>();
            if (cart == null)
            {
                throw new ArgumentException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(Cart)} as an argument");
            }
        }
    }
}