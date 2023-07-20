using System.Collections.Generic;

namespace Sequence.ABI
{
    public class FunctionAbi
    {
        public Dictionary<string, List<(string[], string)>> Abi;

        public FunctionAbi(Dictionary<string, List<(string[], string)>> abi)
        {
            Abi = abi;
        }

        public List<(string[], string)> GetAbiForFunction(string functionName)
        {
            return this.Abi[functionName];
        }
    }
}