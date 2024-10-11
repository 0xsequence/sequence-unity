using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.Integrations.Sardine
{
    [JsonConverter(typeof(EnumConverter<SardinePaymentType>))]
    internal enum SardinePaymentType
    {
        ach,
        debit,
        credit,
        us_debit,
        international_debit,
        international_credit,
    }
}