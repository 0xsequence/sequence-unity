using Sequence.Utils;

namespace Sequence.Demo
{
    public class QrCodeParams
    {
        public Address PaymentToken;
        public Chain Chain;
        public Address DestinationWallet;
        public string Amount;

        public QrCodeParams(Address paymentToken, Chain chain, Address destinationWallet, string amount = "")
        {
            PaymentToken = paymentToken;
            Chain = chain;
            DestinationWallet = destinationWallet;
            Amount = amount;
            if (string.IsNullOrWhiteSpace(Amount))
            {
                Amount = DecimalNormalizer.Normalize(1);
            }
        }
    }
}