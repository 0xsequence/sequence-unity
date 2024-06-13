using System.Numerics;
using NUnit.Framework;
using Sequence.Contracts;
using UnityEditor.VersionControl;

namespace Sequence.Ethereum.Tests
{
    public class ContractDeployerTests
    {
        // Test cases from https://ethereum.stackexchange.com/questions/760/how-is-the-address-of-an-ethereum-contract-computed
        [TestCase(0, "0x6ac7ea33f8831ea9dcc53393aaa88b25a785dbf0", "0xcd234a471b72ba2f1ccf0a70fcaba648a5eecd8d")]
        [TestCase(1, "0x6ac7ea33f8831ea9dcc53393aaa88b25a785dbf0", "0x343c43a37d37dff08ae8c4a11544c718abb4fcf8")]
        [TestCase(2, "0x6ac7ea33f8831ea9dcc53393aaa88b25a785dbf0", "0xf778b86fa74e846c4f0a1fbd1335fe81c00a0c91")]
        [TestCase(3, "0x6ac7ea33f8831ea9dcc53393aaa88b25a785dbf0", "0xfffd933a0bc612844eaf0c6fe3e5b8e9b6c1d19c")]
        public void TestCalculateContractAddress(int nonce, string deployerAddress, string expected)
        {
            string result = ContractDeployer.CalculateContractAddress(nonce, deployerAddress);
            Assert.AreEqual(expected, result);
        }
    }
}
