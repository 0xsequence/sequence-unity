using NUnit.Framework;
using Sequence;
using Sequence.Utils;

namespace Sequence.Utils.Tests
{
    public class ObjectArrayExtensionTests
    {
        private static object[] AppendObjectTestCases = new object[]
        {
            new object[] { new object[] { 1, "house", true }, Chain.Ethereum, new object[] { 1, "house", true, Chain.Ethereum }},
            new object[] { new object[] { 1, "house", true }, new object[] {1,2,3}, new object[] { 1, "house", true, new object[] {1,2,3} }},
            new object[] { new object[] { 1, "house", true }, new int[] {1,2,3}, new object[] { 1, "house", true, new int[] {1,2,3} }},
            new object[] { new object[] {  }, true, new object[] { true }},
            new object[] { null, true, new object[] { true }},
        };
        
        [TestCaseSource(nameof(AppendObjectTestCases))]
        public void AppendObjectTest<T>(object[] arr, T obj, object[] expected)
        {
            object[] result = arr.AppendObject(obj);
            CollectionAssert.AreEqual(expected, result);
        }

        private static object[] AppendArrayTestCases = new object[]
        {
            new object[] { new object[] {1,2,3}, new object[] {4,5,6}, new object[] {1,2,3,4,5,6}},
            new object[] { new object[] {1,2,3}, new object[] {true}, new object[] {1,2,3,true}},
            new object[] { new object[] {1,2,3}, new object[] {}, new object[] {1,2,3}},
            new object[] { new object[] {}, new object[] {4,5,6}, new object[] {4,5,6}},
            new object[] { new object[] {1,2,3}, null, new object[] {1,2,3}},
            new object[] { null, new object[] {4,5,6}, new object[] {4,5,6}},
            new object[] { new object[] {}, new object[] {}, new object[] {}},
            new object[] { null, new object[] {}, new object[] {}},
            new object[] { new object[] {}, null, new object[] {}},
            new object[] { null, null, new object[] {}},
        };
        
        [TestCaseSource(nameof(AppendArrayTestCases))]
        public void AppendArrayTest(object[] arr1, object[] arr2, object[] expected)
        {
            object[] result = arr1.AppendArray(arr2);
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
