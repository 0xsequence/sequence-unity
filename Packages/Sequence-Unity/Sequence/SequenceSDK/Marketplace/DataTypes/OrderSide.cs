namespace Sequence.Marketplace
{
    public enum OrderSide
    {
        unkown,
        listing,
        offer,
    }
    
    public static class OrderSideExtensions
    {
        public static string AsString(this OrderSide side)
        {
            return side.ToString();
        }
    }
}