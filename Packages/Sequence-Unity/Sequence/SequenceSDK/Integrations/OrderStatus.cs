namespace Sequence.Marketplace
{
    public enum OrderStatus
    {
        unknown,
        active,
        inactive,
        expired,
        cancelled,
        filled,
    }
    
    public static class OrderStatusExtensions
    {
        public static string AsString(this OrderStatus status)
        {
            return status.ToString();
        }
    }
}