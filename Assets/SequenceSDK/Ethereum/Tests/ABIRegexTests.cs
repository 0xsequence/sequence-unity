using NUnit.Framework;
using Sequence.Contracts;
using Assert = UnityEngine.Assertions.Assert;

namespace Sequence.Ethereum.Tests
{
    public class ABIRegexTests
    {
        [TestCase("", false)]
        [TestCase("functionName", true)]
        [TestCase("functionName()", false)]
        [TestCase("functionName123", true)]
        [TestCase("functionName_s", true)]
        [TestCase("function-Name", true)]
        [TestCase("functionName ", false)]
        public void TestMatchesFunctionName(string input, bool expected)
        {
            bool result = ABIRegex.MatchesFunctionName(input);
            Assert.AreEqual(expected, result);
        }

        [TestCase("", false)]
        [TestCase("functionName", false)]
        [TestCase("functionName(", false)]
        [TestCase("functionName()", true)]
        [TestCase("functionName(a)", true)]
        [TestCase("functionName(a a)", false)]
        [TestCase("functionName(a, a)", true)]
        [TestCase("functionName(a,a)", true)]
        [TestCase("functionName(,)", false)]
        [TestCase("functionName(Aa123)", true)]
        [TestCase("functionName(a,)", false)]
        [TestCase("functionName() ", false)]
        [TestCase("function_-123Name()", true)]
        public void TestMatchesFunctionABI(string input, bool expected)
        {
            bool result = ABIRegex.MatchesFunctionABI(input);
            Assert.AreEqual(expected, result);
        }
    }
}