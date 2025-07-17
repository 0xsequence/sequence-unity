using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Sequence.EmbeddedWallet;
using UnityEngine;
using UnityEngine.TestTools;

namespace Sequence.WaaS.Tests
{
    public class SequenceLoginTests 
    {
        [Test]
        public void TestSetConnectedWalletAddressNull()
        {
            LogAssert.Expect(LogType.Error, "The connected wallet address cannot be null or empty.");

            Assert.Throws<ArgumentNullException>(
                () => SequenceLogin.GetInstance().SetConnectedWalletAddress(null)
            );
        }
    }
}
