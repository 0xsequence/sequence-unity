using System.Numerics;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;

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

        private class ExtraComplexObjectWithComplexObjectArray
        {
            public string LastValue;
            public ComplexObjectInNonAlphabeticalOrder[] ComplexObjectArray;
            public string HalfwayValue;
            
            public ExtraComplexObjectWithComplexObjectArray(string lastValue, ComplexObjectInNonAlphabeticalOrder[] complexObjectArray, string halwayValue)
            {
                LastValue = lastValue;
                ComplexObjectArray = complexObjectArray;
                HalfwayValue = halwayValue;
            }
        }

        [Test]
        public void TestDelayedEncodeDataArgsGetsSerializedAlphabetically()
        {
            DelayedEncodeData testData = new DelayedEncodeData("testAbi(string,ComplexObjectInNonAlphabeticalOrder,int)", 
                new object[] { "some string", new ComplexObjectInNonAlphabeticalOrder("last", "first", "halfway"), 
                BigInteger.One, 5, new object[] { 1, new ComplexObjectInNonAlphabeticalOrder("last", "first", "halfway"), null, "banana" },
                new ComplexObjectInNonAlphabeticalOrder[] { new ("last", "first", "halfway"), null },
                new object[] { 1, new object[] { 2, new ComplexObjectInNonAlphabeticalOrder("last", "first", "halfway"), 3, 
                    new ExtraComplexObjectWithComplexObjectArray("last", new ComplexObjectInNonAlphabeticalOrder[] { null, 
                        new ("last", "first", "halfway")}, "halfway")}, "word"}
                }, "testFunc");
            
            string json = JsonConvert.SerializeObject(testData);
            Debug.Log(json);
            
            Assert.AreEqual("{\"abi\":\"testAbi(string,ComplexObjectInNonAlphabeticalOrder,int)\",\"args\":[\"some string\",{\"FirstValue\":\"first\",\"HalfwayValue\":\"halfway\",\"LastValue\":\"last\"},1,5,[1,{\"FirstValue\":\"first\",\"HalfwayValue\":\"halfway\",\"LastValue\":\"last\"},null,\"banana\"],[{\"FirstValue\":\"first\",\"HalfwayValue\":\"halfway\",\"LastValue\":\"last\"},null],[1,[2,{\"FirstValue\":\"first\",\"HalfwayValue\":\"halfway\",\"LastValue\":\"last\"},3,{\"ComplexObjectArray\":[null,{\"FirstValue\":\"first\",\"HalfwayValue\":\"halfway\",\"LastValue\":\"last\"}],\"HalfwayValue\":\"halfway\",\"LastValue\":\"last\"}],\"word\"]],\"func\":\"testFunc\"}", json);
        }
    }
}