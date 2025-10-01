using System.Numerics;
using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet.Envelope
{
    public class Envelope<T> where T : Payload
    {
        public Address wallet;
        public BigInteger chainId;
        public Primitives.Config configuration;
        public T payload;

        public SignedEnvelope<T> ToSigned(EnvelopeSignature signature)
        {
            return new SignedEnvelope<T>
            {
                wallet = wallet,
                chainId = chainId,
                configuration = configuration,
                payload = payload,
                signatures = new[] { signature },
            };
        }

        public bool ReachedThreshold()
        {
            var result = WeightOf();
            return result.weight >= result.maxWeight;
        }

        public WeightCalculator.WeightResult WeightOf()
        {
            return WeightCalculator.GetWeight(configuration, (_) => false);
        }
    }
}