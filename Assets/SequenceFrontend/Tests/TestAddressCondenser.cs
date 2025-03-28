using NUnit.Framework;
using Sequence;
using Sequence.Demo;

namespace SequenceExamples.Scripts.Tests
{
    public class TestAddressCondenser
    {
        private static object[] CondenseForUITestCases = new[]
        {
            new object[] {new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), "0xc68...249"},
            new object[] {new Address("0x1099542D7dFaF6757527146C0aB9E70A967f71C0"), "0x109...1C0"},
            new object[] {new Address("0x606e6d28e9150D8A3C070AEfB751a2D0C5DB19fa"), "0x606...9fa"}
        };
        
        [TestCaseSource(nameof(CondenseForUITestCases))]
        public void TestCondenseForUI(Address address, string expected)
        {
            string actual = address.CondenseForUI();
            Assert.AreEqual(expected, actual);
        }
    }
}