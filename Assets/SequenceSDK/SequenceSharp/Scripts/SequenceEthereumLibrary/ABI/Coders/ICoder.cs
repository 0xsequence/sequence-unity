namespace Sequence.ABI
{
    public interface ICoder
    {
        byte[] Encode(object value);
        //string EncodeToString(object value);
        object Decode(byte[] encoded);
        //string DecodeToString(byte[] encoded);
        T DefaultValue<T>();
    }
}