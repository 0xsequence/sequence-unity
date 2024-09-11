using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class ListCollectibleOffersReturn
    {
        public Order[] offers;
        public Page page;

        public ListCollectibleOffersReturn(Order[] offers, Page page)
        {
            this.offers = offers;
            this.page = page;
        }
    }
}