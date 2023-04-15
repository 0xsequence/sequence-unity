using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SequenceSharp.RLP;
using SequenceSharp.ABI;
using System.Linq;
using System.Text;

public class RLPTest
{
    [Test]
    public void RLPEncodingStandardTests()
    {
        //According to examples in following 
        //https://ethereum.org/en/developers/docs/data-structures-and-encoding/rlp/

        byte[] dog_expected = new byte[]{ 0x83, (byte)'d', (byte)'o', (byte)'g' };
        byte[] dog_encoded = RLP.Encode(Encoding.UTF8.GetBytes("dog"));
        
        Debug.Log(SequenceCoder.ByteArrayToHexString(dog_expected));
        Debug.Log(SequenceCoder.ByteArrayToHexString(dog_encoded));
        CollectionAssert.AreEqual(dog_expected, dog_encoded);

        byte[] list_expected = new byte[] { 0xc8, 0x83, (byte)'c', (byte)'a', (byte)'t', 0x83, (byte)'d', (byte)'o', (byte)'g' };
        byte[] list_encoded = RLP.Encode(new List<object>() { Encoding.UTF8.GetBytes("cat"), Encoding.UTF8.GetBytes("dog") });
        Debug.Log(SequenceCoder.ByteArrayToHexString(list_expected));
        Debug.Log(SequenceCoder.ByteArrayToHexString(list_encoded));
        CollectionAssert.AreEqual(list_expected, list_encoded);

        byte[] helloworld_expected = new byte[] { 0xcc, 0x85, 0x68, 0x65, 0x6c, 0x6c, 0x6f, 0x85, 0x77, 0x6f, 0x72, 0x6c, 0x64 };
        byte[] helloworld_encoded = RLP.Encode(new List<object>() { Encoding.UTF8.GetBytes("hello"), Encoding.UTF8.GetBytes("world") });
        Debug.Log(SequenceCoder.ByteArrayToHexString(helloworld_expected));
        Debug.Log(SequenceCoder.ByteArrayToHexString(helloworld_encoded));
        CollectionAssert.AreEqual(helloworld_expected, helloworld_encoded);


        string a_1024 = new string('a', 1024);
        byte[] a_1024_expected = new byte[] { 0xb9, 0x04, 0x00 };
        a_1024_expected = a_1024_expected.Concat(Enumerable.Repeat((byte)'a', 1024).ToArray()).ToArray();
        byte[] a_1024_encoded = RLP.Encode(Encoding.UTF8.GetBytes(a_1024));
        
        Debug.Log(SequenceCoder.ByteArrayToHexString(a_1024_expected));
        Debug.Log(SequenceCoder.ByteArrayToHexString(a_1024_encoded));
        CollectionAssert.AreEqual(a_1024_expected, a_1024_encoded);

        //List<object> longList = new List<object>(1024);
        List<object> longList = Enumerable.Repeat((object)(Encoding.UTF8.GetBytes("a")), 1024).ToList();
        byte[] longList_expected = new byte[] { 0xf9, 0x04, 0x00 };
        longList_expected = longList_expected.Concat(Enumerable.Repeat((byte)'a', 1024).ToArray()).ToArray();
        byte[] longList_encoded = RLP.Encode(longList);

        Debug.Log(SequenceCoder.ByteArrayToHexString(longList_expected));
        Debug.Log(SequenceCoder.ByteArrayToHexString(longList_encoded));
        CollectionAssert.AreEqual(longList_expected, longList_encoded);
    }

    [Test]
    public void RLPDecodingStandardTests()
    {
        byte[] dog_encoded= new byte[] { 0x83, (byte)'d', (byte)'o', (byte)'g' };
        string dog_expected = "dog";
        string dog_decoded = (string)RLP.Decode(dog_encoded);
        Debug.Log(dog_expected);
        Debug.Log(dog_decoded);
        CollectionAssert.AreEqual(dog_expected, dog_decoded);

        byte[] list_encoded = new byte[] { 0xc8, 0x83, (byte)'c', (byte)'a', (byte)'t', 0x83, (byte)'d', (byte)'o', (byte)'g' };
        List<object> list_expected = new List<object>() { "cat", "dog" };
        List<object> list_decoded = (List<object>)RLP.Decode(list_encoded);
        foreach(object item in list_expected)
        {
            Debug.Log(item);
        }
        foreach (object item in list_decoded)
        {
            Debug.Log(item);
        }
        CollectionAssert.AreEqual(list_expected, list_decoded);

        byte[] helloworld_encoded = new byte[] { 0xcc, 0x85, 0x68, 0x65, 0x6c, 0x6c, 0x6f, 0x85, 0x77, 0x6f, 0x72, 0x6c, 0x64 };
        List<object> helloworld_expected = new List<object>() { "hello", "world" };
        List<object> helloworld_decoded =(List<object>) RLP.Decode(helloworld_encoded);
        foreach (object item in helloworld_expected)
        {
            Debug.Log(item);
        }
        foreach (object item in helloworld_decoded)
        {
            Debug.Log(item);
        }
        CollectionAssert.AreEqual(helloworld_expected, helloworld_decoded);

        string a_1024 = new string('a', 1024);        
        byte[] a_1024_encoded = RLP.Encode(Encoding.UTF8.GetBytes(a_1024));
        string a_1024_decoded = (string)RLP.Decode(a_1024_encoded);
        Debug.Log(a_1024);
        Debug.Log(a_1024_decoded);
        CollectionAssert.AreEqual(a_1024, a_1024_decoded);

        List<object> longList = Enumerable.Repeat((object)(Encoding.UTF8.GetBytes("a")), 1024).ToList();      
        byte[] longList_encoded = RLP.Encode(longList);
        List<object> longList_decoded = (List<object>)RLP.Decode(longList_encoded);
        /*foreach (object item in longList)
        {
            Debug.Log(item);
        }
        foreach (object item in longList_decoded)
        {
            Debug.Log(item);
        }*/

        CollectionAssert.AreEqual(longList, longList_decoded);


    }

        [Test]
    public void RLPEncodingEmptyTests()
    {

    }

    [Test]
    public void RLPEncodingBooleanTests()
    {

    }

    [Test]
    public void RLPEncodingStringTests()
    {

    }

    [Test]
    public void RLPEncodingIntegerTests()
    {

    }

    [Test]
    public void RLPEncodingNegativeIntegerTests()
    {

    }

    [Test]
    public void RLPEncodingByteArrayTests()
    {

    }


    [Test]
    public void RLPEncodingBigIntegerTests()
    {

    }

    [Test]
    public void RLPDecodingTests()
    {

    }
}
