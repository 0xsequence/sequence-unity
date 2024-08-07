namespace Sequence.Marketplace
{
    public enum PropertyType
    {
        INT,
        STRING,
        ARRAY,
        GENERIC,
    }
    
    public static class PropertyTypeExtensions
    {
        public static string AsString(this PropertyType propertyType)
        {
            return propertyType.ToString();
        }
    }
}