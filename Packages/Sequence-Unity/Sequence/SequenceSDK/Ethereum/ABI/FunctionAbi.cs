using System;
using System.Collections.Generic;
using System.Text;
using Sequence.Utils;

namespace Sequence.ABI
{
    public class FunctionAbi
    {
        public Dictionary<string, List<(string[], string)>> Abi;

        public FunctionAbi(Dictionary<string, List<(string[], string)>> abi)
        {
            Abi = abi;
        }

        public List<(string[], string)> GetAbisForFunction(string functionName)
        {
            if (!this.Abi.ContainsKey(functionName))
            {
                return new List<(string[], string)>();
            }
            return this.Abi[functionName];
        }

        public string GetFunctionSignature(string functionName, int abiIndex = 0)
        {
            return $"{functionName}({string.Join(',', this.Abi[functionName][abiIndex].Item1)})";
        }

        private string[] GetFunctionSignatures(string functionName)
        {
            List<(string[], string)> abis = this.Abi[functionName];
            int abisLength = abis.Count;
            string[] functionSignatures = new string[abisLength];
            for (int i = 0; i < abisLength; i++)
            {
                functionSignatures[i] = GetFunctionSignature(functionName, i);
            }

            return functionSignatures;
        }

        /// <summary>
        /// Used to determine which version of a function signature you wish to call
        /// Throws if you have provided an invalid number of args to a function name (there doesn't exist a matching function signature in the abi)
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public int GetFunctionAbiIndex(string functionName, params object[] args)
        {
            // In Solidity, function overloading is based on the number of parameters.
            // We can distinguish which Abi the user is looking to call based on the number of arguments/parameters provided.
            List<(string[], string)> functionAbis = GetAbisForFunction(functionName);
            int functionAbisLength = functionAbis.Count;
            if (functionAbisLength == 0)
            {
                throw new ArgumentException($"Invalid function \'{functionName}\' does not exist in contract ABI");
            }
            int argsLength = 0;
            if (args != null)
            {
                argsLength = args.Length;
            }
            for (int i = 0; i < functionAbisLength; i++)
            {
                if (functionAbis[i].Item1.Length == argsLength)
                {
                    return i;
                }
            }

            throw new ArgumentException(
                $"Invalid function arguments for \'{functionName}\' are invalid. Given: \'{args.ExpandToString()}\' Valid function signatures: \'{string.Join(", ", GetFunctionSignatures(functionName))}\'");
        }

        public bool IsEqualTo(FunctionAbi other)
        {
            if (this.Abi.Count != other.Abi.Count)
            {
                return false;
            }

            foreach (string function in this.Abi.Keys)
            {
                if (!other.Abi.ContainsKey(function))
                {
                    return false;
                }

                if (!SignatureIsEqualTo(this.Abi[function], other.Abi[function]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool SignatureIsEqualTo(List<(string[], string)> a, List<(string[], string)> b)
        {
            int thisAbisLength = a.Count;
            int otherAbisLength = b.Count;
            if (thisAbisLength != otherAbisLength)
            {
                return false;
            }

            for (int i = 0; i < thisAbisLength; i++)
            {
                int thisParamsLength = a[i].Item1.Length;
                int otherParamsLength = b[i].Item1.Length;
                if (thisParamsLength != otherParamsLength)
                {
                    return false;
                }

                for (int j = 0; j < thisParamsLength; j++)
                {
                    if (a[i].Item1[j] != b[i].Item1[j])
                    {
                        return false;
                    }
                }

                if (a[i].Item2 != b[i].Item2)
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            if (Abi == null || Abi.Count == 0)
            {
                return "FunctionAbi: <Empty>";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("FunctionAbi: ");
            bool first = true;
            foreach (var entry in Abi)
            {
                string functionName = entry.Key;
                if (first)
                {
                    first = false;
                    sb.Append($"Function {functionName}: ");
                }
                else
                {
                    sb.Append($"| Function {functionName}: ");
                }

                sb.Append(SignatureToString(entry.Value));
            }

            return sb.ToString();
        }
    
        public static string SignatureToString(List<(string[], string)> signature) 
        {
            int length = signature.Count;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(
                    $"(Parameters: [{string.Join(", ", signature[i].Item1)}], Return: {signature[i].Item2 ?? "void"}) ");
            }

            return sb.ToString();
        }

        public T DecodeReturnValue<T>(string value, string functionName, params object[] args)
        {
            string returnType = GetReturnType(functionName, args);
            return ABI.Decode<T>(value, returnType);
        }

        private string GetReturnType(string functionName, params object[] args)
        {
            int index = GetFunctionAbiIndex(functionName, args);
            return this.Abi[functionName][index].Item2;
        }
    }
}