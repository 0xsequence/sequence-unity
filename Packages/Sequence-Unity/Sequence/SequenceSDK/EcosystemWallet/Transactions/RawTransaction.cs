using System.Numerics;
using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet
{
    public class RawTransaction : ITransaction
    {
        private readonly Address _to;
        private readonly BigInteger _value;
        private readonly byte[] _data;
        private readonly BigInteger _gasLimit;
        private readonly bool _delegateCall;
        private readonly bool _onlyFallback;
        private readonly BehaviourOnError _behaviorOnError = BehaviourOnError.revert;

        public RawTransaction(Address to, BigInteger value, byte[] data)
        {
            _to = to;
            _value = value;
            _data = data;
        }
        
        public RawTransaction(Address to, BigInteger value, byte[] data, BigInteger gasLimit, bool delegateCall, bool onlyFallback, BehaviourOnError behaviourOnError)
        {
            _to = to;
            _value = value;
            _data = data;
            _gasLimit = gasLimit;
            _delegateCall = delegateCall;
            _onlyFallback = onlyFallback;
            _behaviorOnError = behaviourOnError;
        }

        public Call GetCall()
        {
            return new Call(_to, _value, _data, _gasLimit, _delegateCall, _onlyFallback, _behaviorOnError);
        }
    }
}