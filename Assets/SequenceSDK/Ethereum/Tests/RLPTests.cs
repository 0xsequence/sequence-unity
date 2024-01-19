using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Sequence.RLP;
using Sequence.ABI;
using System.Text;
using Sequence.Extensions;
using Sequence.Utils;

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
        a_1024_expected = ByteArrayExtensions.ConcatenateByteArrays(a_1024_expected, ByteArrayExtensions.BuildArrayWithRepeatedValue(new byte[] {(byte)'a'}, 1024));
        byte[] a_1024_encoded = RLP.Encode(Encoding.UTF8.GetBytes(a_1024));
        
        Debug.Log(SequenceCoder.ByteArrayToHexString(a_1024_expected));
        Debug.Log(SequenceCoder.ByteArrayToHexString(a_1024_encoded));
        CollectionAssert.AreEqual(a_1024_expected, a_1024_encoded);

        //List<object> longList = new List<object>(1024);
        var tempArray = ByteArrayExtensions.BuildArrayWithRepeatedValue(new byte[] { (byte)'a' }, 1024);
        List<object> longList = new List<object>();
        foreach (byte b in tempArray)
        {
            longList.Add(new byte[]{b});
        }
        byte[] longList_expected = new byte[] { 0xf9, 0x04, 0x00 };
        longList_expected = ByteArrayExtensions.ConcatenateByteArrays(longList_expected, ByteArrayExtensions.BuildArrayWithRepeatedValue(new byte[] { (byte)'a' }, 1024));
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
        
        var tempArray = ByteArrayExtensions.BuildArrayWithRepeatedValue(new byte[] { (byte)'a' }, 1024);
        List<object> longList = new List<object>();
        foreach (byte b in tempArray)
        {
            longList.Add(new byte[]{b});
        }
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
        //Empty string
        byte[] empty_string_expected = new byte[] { 0x80 };
        byte[] empty_string_encoded = RLP.Encode(Encoding.UTF8.GetBytes(""));

        Debug.Log(SequenceCoder.ByteArrayToHexString(empty_string_expected));
        Debug.Log(SequenceCoder.ByteArrayToHexString(empty_string_encoded));
        CollectionAssert.AreEqual(empty_string_expected, empty_string_encoded);

        //Empty list
        byte[] empty_list_expected = new byte[] { 0xc0 };
        byte[] empty_list_encoded = RLP.Encode(new List<object>());

        Debug.Log(SequenceCoder.ByteArrayToHexString(empty_list_expected));
        Debug.Log(SequenceCoder.ByteArrayToHexString(empty_list_encoded));
        CollectionAssert.AreEqual(empty_list_expected, empty_list_encoded);

        //Set theoretical representation of three
        byte[] three_expected = new byte[] { 0xc7, 0xc0, 0xc1, 0xc0, 0xc3, 0xc0, 0xc1, 0xc0 };
        byte[] three_encoded = RLP.Encode(new List<object>() { new List<object>() { }, new List<object>() { new List<object>() { } }, new List<object>() { new List<object>() { }, new List<object>() { new List<object>() { } } } });
        Debug.Log(SequenceCoder.ByteArrayToHexString(three_expected));
        Debug.Log(SequenceCoder.ByteArrayToHexString(three_encoded));
        CollectionAssert.AreEqual(three_expected, three_encoded);
    }
    [Test]
    public void RLPDecodingEmptyTests()
    {
        //Empty string
        string empty_string_expected = "";
        string empty_string_decoded = (string)RLP.Decode(new byte[] { 0x80 });
        Debug.Log(empty_string_expected);
        Debug.Log(empty_string_decoded);
        CollectionAssert.AreEqual(empty_string_expected, empty_string_decoded);

        //Empty list
        List<object> empty_list_expected = new List<object>();
        List<object> empty_list_decoded =(List<object>)RLP.Decode(new byte[] { 0xc0 });

        Debug.Log(empty_list_expected.Count);
        Debug.Log(empty_list_decoded.Count);
        CollectionAssert.AreEqual(empty_list_expected, empty_list_decoded);

        //Set theoretical representation of three
        List<object> three_expected = new List<object>() { new List<object>() { }, new List<object>() { new List<object>() { } }, new List<object>() { new List<object>() { }, new List<object>() { new List<object>() { } } } };//;
        List<object> three_decoded = (List<object>)RLP.Decode(new byte[] { 0xc7, 0xc0, 0xc1, 0xc0, 0xc3, 0xc0, 0xc1, 0xc0 });
        Debug.Log(three_expected.Count);
        Debug.Log(three_decoded.Count);
        foreach (object item in three_expected)
        {
            Debug.Log("expected: "+ item);
        }
        foreach (object item in three_decoded)
        {
            Debug.Log("decoded: "+ item);
        }
        CollectionAssert.AreEqual(three_expected, three_decoded);
    }

        [Test]
    public void RLPEncodingBooleanTests()
    {
        //True
        Assert.AreEqual(new byte[] { 0x01 }, RLP.Encode(true));
        //False
        Assert.AreEqual(new byte[] { 0x80 }, RLP.Encode(false));
    }


    [Test]
    public void RLPEncodingStringTests()
    {
        byte[] string_1_expected = new byte[] 
                {   0xb8,
                    0x38,
                    (byte)'L',
                    (byte)'o',
                    (byte)'r',
                    (byte)'e',
                    (byte)'m',
                    (byte)' ',
                    (byte)'i',
                    (byte)'p',
                    (byte)'s',
                    (byte)'u',
                    (byte)'m',
                    (byte)' ',
                    (byte)'d',
                    (byte)'o',
                    (byte)'l',
                    (byte)'o',
                    (byte)'r',
                    (byte)' ',
                    (byte)'s',
                    (byte)'i',
                    (byte)'t',
                    (byte)' ',
                    (byte)'a',
                    (byte)'m',
                    (byte)'e',
                    (byte)'t',
                    (byte)',',
                    (byte)' ',
                    (byte)'c',
                    (byte)'o',
                    (byte)'n',
                    (byte)'s',
                    (byte)'e',
                    (byte)'c',
                    (byte)'t',
                    (byte)'e',
                    (byte)'t',
                    (byte)'u',
                    (byte)'r',
                    (byte)' ',
                    (byte)'a',
                    (byte)'d',
                    (byte)'i',
                    (byte)'p',
                    (byte)'i',
                    (byte)'s',
                    (byte)'i',
                    (byte)'c',
                    (byte)'i',
                    (byte)'n',
                    (byte)'g',
                    (byte)' ',
                    (byte)'e',
                    (byte)'l',
                    (byte)'i',
                    (byte)'t'
                };
        byte[] string_1_encoded = RLP.Encode(Encoding.UTF8.GetBytes("Lorem ipsum dolor sit amet, consectetur adipisicing elit"));

        CollectionAssert.AreEqual(string_1_expected, string_1_encoded);


    }

    [Test]
    public void RLPEncodingIntegerTests()
    {
        byte[] integer_1_expected = new byte[] { 0x82, 0x04, 0x00 };
        int integer_1 = 0x400;
        string integer_1_str = integer_1.ToString("X");
        Debug.Log(integer_1_str);
        byte[] integer_1_encoded = RLP.Encode(SequenceCoder.HexStringToByteArray(integer_1_str));
        Debug.Log(SequenceCoder.ByteArrayToHexString(integer_1_encoded));
        CollectionAssert.AreEqual(integer_1_expected, integer_1_encoded);
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
