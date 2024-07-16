using System;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using Sequence.ABI;
using Sequence.Contracts;
using Sequence.Wallet;

namespace Sequence.Ethereum.Tests
{
        public class FunctionAbiTests
        {
                private static EOAWallet wallet1 = new EOAWallet("0xabc0000000000000000000000000000000000000000000000000000000000001");

                private static readonly object[] GetAbisForFunctionTests = 
                {
                        new object[] {"name", new List<(string[], string)> { (new string[] { }, "(string)") }},
                        new object[] {"safeTransferFrom", new List<(string[], string)> { (new string[] { "address", "address", "uint256" }, null), (new string[] { "address", "address", "uint256", "bytes" }, null) }},
                        new object[] {"something random that isn't on there", new List<(string[], string)>()},
                };
                [TestCaseSource(nameof(GetAbisForFunctionTests))]
                public void TestGetAbisForFunction(string functionName, List<(string[], string)> expected)
                {
                        FunctionAbi abi = ABI.ABI.DecodeAbi(ERC721.Abi);
                        var result = abi.GetAbisForFunction(functionName);
                        Assert.True(FunctionAbi.SignatureIsEqualTo(expected, result), 
                                $"Not equal. Expected: {FunctionAbi.SignatureToString(expected)} Received: {FunctionAbi.SignatureToString(result)}");
                }

                private static readonly object[] GetFunctionSignatureTests =
                {
                        new object[] { "name", 0, "name()" },
                        new object[] { "safeTransferFrom", 0, "safeTransferFrom(address,address,uint256)" },
                        new object[] { "safeTransferFrom", 1, "safeTransferFrom(address,address,uint256,bytes)" },
                };
                [TestCaseSource(nameof(GetFunctionSignatureTests))]
                public void TestGetFunctionSignature(string functionName, int abiIndex, string expected)
                {
                        FunctionAbi abi = ABI.ABI.DecodeAbi(ERC721.Abi);
                        string result = abi.GetFunctionSignature(functionName, abiIndex);
                        Assert.AreEqual(expected, result);
                }

                private static readonly object[] GetFunctionAbiIndexTests =
                {
                        new object[] { 0, "name", null },
                        new object[] { 0, "safeTransferFrom",  new object[] {wallet1.GetAddress(), wallet1.GetAddress(), BigInteger.One}},
                        new object[] { 1, "safeTransferFrom",  new object[] {wallet1.GetAddress(), wallet1.GetAddress(), BigInteger.One, new byte[3]}},
                };
                [TestCaseSource(nameof(GetFunctionAbiIndexTests))]
                public void TestGetFunctionAbiIndex(int expected, string functionName, params object[] args)
                {
                        FunctionAbi abi = ABI.ABI.DecodeAbi(ERC721.Abi);
                        int result;
                        if (args != null)
                        {
                                result = abi.GetFunctionAbiIndex(functionName, args);
                        }
                        else
                        {
                                result = abi.GetFunctionAbiIndex(functionName); 
                        }
                        Assert.AreEqual(expected, result);
                }

                [Test]
                public void TestGetFunctionAbiIndex_throwsOnInvalidInput()
                {
                        FunctionAbi abi = ABI.ABI.DecodeAbi(ERC721.Abi);
                        try
                        {
                                int result = abi.GetFunctionAbiIndex("name", "random stuff");
                                Assert.Fail("Expected exception but none was thrown");
                        }
                        catch (Exception ex)
                        {
                                Assert.AreEqual($"Invalid function arguments for \'name\' are invalid. Given: \'random stuff\' Valid function signatures: \'name()\'", ex.Message);
                        }
                        
                        try
                        {
                                int result = abi.GetFunctionAbiIndex("safeTransferFrom", "random stuff");
                                Assert.Fail("Expected exception but none was thrown");
                        }
                        catch (Exception ex)
                        {
                                Assert.AreEqual($"Invalid function arguments for \'safeTransferFrom\' are invalid. Given: \'random stuff\' Valid function signatures: \'safeTransferFrom(address,address,uint256), safeTransferFrom(address,address,uint256,bytes)\'", ex.Message);
                        }
                        
                        
                        try
                        {
                                int result = abi.GetFunctionAbiIndex("invalid name", "random stuff");
                                Assert.Fail("Expected exception but none was thrown");
                        }
                        catch (Exception ex)
                        {
                                Assert.AreEqual($"Invalid function \'invalid name\' does not exist in contract ABI", ex.Message);
                        }
                }
        }
}
