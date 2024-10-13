using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Sequence;
using Sequence.Utils;

namespace Sequence.Utils.Tests
{
    public class ArrayUtilsTests
    {
        public static IEnumerable<TestCaseData> ConvertToTArrayTestCases()
        {
            yield return new TestCaseData(new object[] { 1, 2, 3 }, new int[] { 1, 2, 3 });
            List<object> list = new List<object>();
            list.Add("banana");
            list.Add("SDK by Horizon");
            list.Add("some more stuff");
            yield return new TestCaseData(list, new string[] { "banana", "SDK by Horizon", "some more stuff" });
            Dictionary<Chain, string> chains = new Dictionary<Chain, string>();
            chains[Chain.Ethereum] = "ETH";
            chains[Chain.Avalanche] = "AVAX";
            chains[Chain.Polygon] = "POL";
            yield return new TestCaseData(chains.Keys.GetEnumerator(), new Chain[] { Chain.Ethereum, Chain.Avalanche, Chain.Polygon });
        }
            
        [TestCaseSource(nameof(ConvertToTArrayTestCases))]
        public void TestConvertToTArray(object input, Array expected)
        {
            Type T = expected.GetType().GetElementType();
            Type T2 = input.GetType();
            MethodInfo method = typeof(ArrayUtilsTests).GetMethod(nameof(ConvertToTArray), BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo genericMethod = method.MakeGenericMethod(T, T2);
            
            object result = genericMethod.Invoke(null, new object[] { input });
            
            CollectionAssert.AreEqual(expected, (Array)result);
        }

        private static T[] ConvertToTArray<T, T2>(T2 value)
        {
            return value.ConvertToTArray<T, T2>();
        }
    }
}
