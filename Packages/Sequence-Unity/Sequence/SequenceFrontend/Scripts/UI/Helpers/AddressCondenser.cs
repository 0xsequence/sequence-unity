namespace Sequence.Demo
{
    public static class AddressCondenser
    {
        public static string CondenseForUI(this Address address)
        {
            return address.ToString().Substring(0, 6) + "..." + address.ToString().Substring(38, 4);
        }
    }
}