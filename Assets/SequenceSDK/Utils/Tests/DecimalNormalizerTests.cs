using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Sequence.Utils;

public class DecimalNormalizerTests
{
    [Test]
    public void Normalize_WithDefaultDecimals_ReturnsCorrectString()
    {
        string result = DecimalNormalizer.Normalize(1.5f);
        Assert.AreEqual("1500000000000000000", result);
    }

    [Test]
    public void Normalize_WithCustomDecimals_ReturnsCorrectString()
    {
        string result = DecimalNormalizer.Normalize(1.5f, 6);
        Assert.AreEqual("1500000", result);
    }

    [Test]
    public void Normalize_WithZero_ReturnsZero()
    {
        string result = DecimalNormalizer.Normalize(0f);
        Assert.AreEqual("0", result);
    }

    [Test]
    public void Normalize_WithNegativeNumber_ReturnsPositiveResult()
    {
        string result = DecimalNormalizer.Normalize(-1.5f, 6);
        Assert.AreEqual("1500000", result);
    }

    [Test]
    public void NormalizeAsBigInteger_WithDefaultDecimals_ReturnsCorrectBigInteger()
    {
        BigInteger result = DecimalNormalizer.NormalizeAsBigInteger(1.5f);
        BigInteger expected = new BigInteger(1500000000000000000);
        Assert.AreEqual(expected, result);
    }

    [Test]
    public void NormalizeAsBigInteger_WithCustomDecimals_ReturnsCorrectBigInteger()
    {
        BigInteger result = DecimalNormalizer.NormalizeAsBigInteger(2.75f, 4);
        BigInteger expected = new BigInteger(27500);
        Assert.AreEqual(expected, result);
    }

    [Test]
    public void NormalizeAsBigInteger_WithSmallNumber_ReturnsCorrectBigInteger()
    {
        BigInteger result = DecimalNormalizer.NormalizeAsBigInteger(0.001f, 6);
        BigInteger expected = new BigInteger(1000);
        Assert.AreEqual(expected, result);
    }

    [Test]
    public void ReturnToNormalString_WithWholeNumber_ReturnsCorrectString()
    {
        BigInteger input = new BigInteger(1500000000000000000);
        string result = DecimalNormalizer.ReturnToNormalString(input, 18);
        Assert.AreEqual("1.5", result);
    }

    [Test]
    public void ReturnToNormalString_WithCustomDecimals_ReturnsCorrectString()
    {
        BigInteger input = new BigInteger(1500000);
        string result = DecimalNormalizer.ReturnToNormalString(input, 6);
        Assert.AreEqual("1.5", result);
    }

    [Test]
    public void ReturnToNormalString_WithSmallNumber_ReturnsCorrectString()
    {
        BigInteger input = new BigInteger(1000);
        string result = DecimalNormalizer.ReturnToNormalString(input, 6);
        Assert.AreEqual("0.001", result);
    }

    [Test]
    public void ReturnToNormalString_WithZero_ReturnsZero()
    {
        BigInteger input = new BigInteger(0);
        string result = DecimalNormalizer.ReturnToNormalString(input, 18);
        Assert.AreEqual("0", result);
    }

    [Test]
    public void ReturnToNormalString_WithNegativeNumber_ReturnsPositiveResult()
    {
        BigInteger input = new BigInteger(-1500000000000000000);
        string result = DecimalNormalizer.ReturnToNormalString(input, 18);
        Assert.AreEqual("1.5", result);
    }

    [Test]
    public void ReturnToNormalString_WithTrailingZeros_TrimsZeros()
    {
        BigInteger input = new BigInteger(1500000000000000000);
        string result = DecimalNormalizer.ReturnToNormalString(input, 18);
        Assert.AreEqual("1.5", result);
    }

    [Test]
    public void ReturnToNormal_WithBigInteger_ReturnsCorrectFloat()
    {
        BigInteger input = new BigInteger(1500000000000000000);
        float result = DecimalNormalizer.ReturnToNormal(input, 18);
        Assert.AreEqual(1.5f, result, 0.0001f);
    }

    [Test]
    public void ReturnToNormal_WithCustomDecimals_ReturnsCorrectFloat()
    {
        BigInteger input = new BigInteger(2750000);
        float result = DecimalNormalizer.ReturnToNormal(input, 6);
        Assert.AreEqual(2.75f, result, 0.0001f);
    }

    [Test]
    public void ReturnToNormalPrecise_WithBigInteger_ReturnsCorrectDecimal()
    {
        BigInteger input = new BigInteger(1500000000000000000);
        decimal result = DecimalNormalizer.ReturnToNormalPrecise(input, 18);
        Assert.AreEqual(1.5m, result);
    }

    [Test]
    public void ReturnToNormalPrecise_WithCustomDecimals_ReturnsCorrectDecimal()
    {
        BigInteger input = new BigInteger(3250000);
        decimal result = DecimalNormalizer.ReturnToNormalPrecise(input, 6);
        Assert.AreEqual(3.25m, result);
    }

    [Test]
    public void RoundTrip_NormalizeAndReturnToNormal_PreservesValue()
    {
        float originalValue = 12.34f;
        int decimals = 6;
        
        BigInteger normalized = DecimalNormalizer.NormalizeAsBigInteger(originalValue, decimals);
        float returned = DecimalNormalizer.ReturnToNormal(normalized, decimals);
        
        Assert.AreEqual(originalValue, returned, 0.0001f);
    }

    [Test]
    public void EdgeCase_VerySmallNumber_HandlesCorrectly()
    {
        float verySmall = 0.000001f;
        int decimals = 18;
        
        string normalized = DecimalNormalizer.Normalize(verySmall, decimals);
        BigInteger bigInt = BigInteger.Parse(normalized);
        float returned = DecimalNormalizer.ReturnToNormal(bigInt, decimals);
        
        Assert.AreEqual(verySmall, returned, 0.0000001f);
    }

    [Test]
    public void EdgeCase_LargeNumber_HandlesCorrectly()
    {
        float largeNumber = 9999.99f;
        int decimals = 6;
        
        BigInteger normalized = DecimalNormalizer.NormalizeAsBigInteger(largeNumber, decimals);
        float returned = DecimalNormalizer.ReturnToNormal(normalized, decimals);
        
        Assert.AreEqual(largeNumber, returned, 0.01f);
    }

    [Test]
    public void BugReproTest_String()
    {
        BigInteger input = BigInteger.Parse("10000000000000000000000000");
        
        string result = DecimalNormalizer.ReturnToNormalString(input, 18);
        
        Assert.AreEqual("10000000", result);
    }

    [Test]
    public void BugReproTest_Float()
    {
        BigInteger input = BigInteger.Parse("10000000000000000000000000");
        
        float result = DecimalNormalizer.ReturnToNormal(input, 18);
        
        Assert.AreEqual(10000000f, result);
    }

    [Test]
    public void BugReproTest_Decimal()
    {
        BigInteger input = BigInteger.Parse("10000000000000000000000000");
        
        decimal result = DecimalNormalizer.ReturnToNormalPrecise(input, 18);
        
        Assert.AreEqual(decimal.Parse("10000000"), result);
    }
}
