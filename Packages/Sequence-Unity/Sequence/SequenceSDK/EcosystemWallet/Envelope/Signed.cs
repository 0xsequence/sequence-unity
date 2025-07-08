using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet.Envelope
{
    public class Signed<T> : Envelope<T> where T : Payload
    {
        public EnvelopeSignature[] signatures;
    }
}