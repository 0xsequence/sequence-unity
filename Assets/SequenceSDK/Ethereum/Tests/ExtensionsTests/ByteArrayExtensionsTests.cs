using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Sequence.Extensions;

public class ByteArrayExtensionsTests 
{
    private static readonly string[][] concatenateByteArrayTestCases = new string[][]
    {
        new string[] {"cow"},
        new string[] {"cow", "horse", "chicken"},
        new string[] {"cow", "turkey", "chicken", "horse", "duck", "pigs,", "and some goats"},
    };

    [TestCaseSource("concatenateByteArrayTestCases")]
    public void TestConcatenateByteArrays(params string[] values)
    {
        byte[] expected = string.Join("", values).ToByteArray();

        int elements = values.Length;
        byte[][] byteArrays = new byte[elements][];
        for (int i = 0; i < elements; i++)
        {
            byteArrays[i] = values[i].ToByteArray();
        }

        byte[] concatenated = ByteArrayExtensions.ConcatenateByteArrays(byteArrays);

        Assert.AreEqual(expected, concatenated);
    }
}
