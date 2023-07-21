using System;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using Sequence.ABI;
using Sequence.Wallet;

public class FunctionAbiTests
{
        private static EthWallet wallet1 = new EthWallet("0xabc0000000000000000000000000000000000000000000000000000000000001");
        private static readonly string erc721Abi = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"burn\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeMint\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";

        private static readonly object[] GetAbisForFunctionTests = 
        {
                new object[] {"name", new List<(string[], string)> { (new string[] { }, "string") }},
                new object[] {"safeTransferFrom", new List<(string[], string)> { (new string[] { "address", "address", "uint256" }, null), (new string[] { "address", "address", "uint256", "bytes" }, null) }},
                new object[] {"something random that isn't on there", new List<(string[], string)>()},
        };
        [TestCaseSource(nameof(GetAbisForFunctionTests))]
        public void TestGetAbisForFunction(string functionName, List<(string[], string)> expected)
        {
                FunctionAbi abi = ABI.DecodeAbi(erc721Abi);
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
                FunctionAbi abi = ABI.DecodeAbi(erc721Abi);
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
                FunctionAbi abi = ABI.DecodeAbi(erc721Abi);
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
                FunctionAbi abi = ABI.DecodeAbi(erc721Abi);
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
        }
}