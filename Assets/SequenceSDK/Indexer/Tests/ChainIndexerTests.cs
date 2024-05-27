using System;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using Sequence;
using Sequence.Utils;

namespace Sequence.Indexer.Tests
{
    public class ChainIndexerTests
    {
        private static List<Chain> chainIdCases = EnumExtensions.GetEnumValuesAsList<Chain>();
    
        [TestCaseSource(nameof(chainIdCases))]
        public void CreateChainIndexerForChain(Chain chain)
        {
            if (chain == Chain.None) return;
            try
            {
                ChainIndexer chainIndexer = new ChainIndexer(chain);
                Assert.AreEqual(chain.GetChainId(), chainIndexer.ChainId);
                Assert.AreEqual(chain, chainIndexer.GetChain());
            }
            catch (Exception e)
            {
                Assert.Fail("Encountered exception when none was expected: " + e.Message);
            }
        }
    }
}