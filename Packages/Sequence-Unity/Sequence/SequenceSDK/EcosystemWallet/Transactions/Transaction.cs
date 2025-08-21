using System.Numerics;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet
{
    public class Transaction : ITransaction
    {
        private readonly Address _to;
        private readonly BigInteger _value;
        private readonly string _functionSignature;
        private readonly object[] _parameters;
        
        public Transaction(Address to, BigInteger value, string functionSignature, params object[] parameters)
        {
            _to = to;
            _value = value;
            _functionSignature = functionSignature;
            _parameters = parameters;
        }
        
        public Call GetCall()
        {
            var data = _parameters is { Length: > 0 } ? 
                ABI.ABI.Pack(_functionSignature, _parameters).HexStringToByteArray() : 
                ABI.ABI.FunctionSelector(_functionSignature).HexStringToByteArray();
            
            return new Call(_to, _value, data);
        }
    }
}