using System.Numerics;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Sequence.EmbeddedWallet.Tests
{
    public class DelayedEncodeJsonSerializationTest
    {
        private class ComplexObjectInNonAlphabeticalOrder
        {
            public string LastValue;
            public string FirstValue;
            public string HalfwayValue;

            public ComplexObjectInNonAlphabeticalOrder(string lastValue, string firstValue, string halfwayValue)
            {
                LastValue = lastValue;
                FirstValue = firstValue;
                HalfwayValue = halfwayValue;
            }
        }

        [Test]
        public void TestDelayedEncodeDataArgsGetsSerializedAlphabetically()
        {
            DelayedEncodeData testData = new DelayedEncodeData("testAbi(string,ComplexObjectInNonAlphabeticalOrder,int)", new object[] { "some string", new ComplexObjectInNonAlphabeticalOrder("last", "first", "halfway"), BigInteger.One, 5 }, "testFunc");
            
            string json = JsonConvert.SerializeObject(testData);
            
            Assert.AreEqual("{\"abi\":\"testAbi(string,ComplexObjectInNonAlphabeticalOrder,int)\",\"args\":[\"some string\",{\"FirstValue\":\"first\",\"HalfwayValue\":\"halfway\",\"LastValue\":\"last\"},1,5],\"func\":\"testFunc\"}", json);
        }
    }
}