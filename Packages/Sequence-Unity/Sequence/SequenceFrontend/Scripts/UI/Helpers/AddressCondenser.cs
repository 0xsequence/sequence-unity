namespace Sequence.Demo
{
    public static class AddressCondenser
    {
        public static string CondenseForUI(this Address address)
        {
            return address.ToString().Substring(0, 5) + "..." + address.ToString().Substring(39, 3);
        }
    }
}