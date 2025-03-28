using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.Pay.Sardine
{
    [JsonConverter(typeof(EnumConverter<SardinePaymentType>))]
    public enum SardinePaymentType
    {
        ach,
        debit,
        credit,
        us_debit,
        international_debit,
        international_credit,
    }
}