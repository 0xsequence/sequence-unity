using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence;
using Sequence.ABI;
using Sequence.Contracts;
using Sequence.Provider;
using Sequence.Utils;
using Sequence.Wallet;
using UnityEngine;

namespace Sequence.Ethereum.Tests
{
    public class ContractIntegrationTests
    {
        private static string complexContractAbi =
            @"[{""inputs"":[{""internalType"":""uint256"",""name"":""number"",""type"":""uint256""},{""internalType"":""string"",""name"":""word"",""type"":""string""},{""internalType"":""uint256[]"",""name"":""numbers"",""type"":""uint256[]""},{""internalType"":""string[][]"",""name"":""doubleNestedWords"",""type"":""string[][]""}],""name"":""testPart1"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""},{""internalType"":""string"",""name"":"""",""type"":""string""},{""internalType"":""uint256[]"",""name"":"""",""type"":""uint256[]""},{""internalType"":""string[][]"",""name"":"""",""type"":""string[][]""}],""stateMutability"":""pure"",""type"":""function""},{""inputs"":[{""internalType"":""bool"",""name"":""boolean"",""type"":""bool""},{""internalType"":""bool[]"",""name"":""bools"",""type"":""bool[]""},{""internalType"":""bytes"",""name"":""byteArray"",""type"":""bytes""},{""internalType"":""bytes32"",""name"":""moreBytes"",""type"":""bytes32""},{""internalType"":""address[]"",""name"":""addresses"",""type"":""address[]""}],""name"":""testPart2"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""},{""internalType"":""bool[]"",""name"":"""",""type"":""bool[]""},{""internalType"":""bytes"",""name"":"""",""type"":""bytes""},{""internalType"":""bytes32"",""name"":"""",""type"":""bytes32""},{""internalType"":""address[]"",""name"":"""",""type"":""address[]""}],""stateMutability"":""pure"",""type"":""function""}]";

        private static string complexContractBytecode =
            "0x608060405234801561001057600080fd5b50610f7d806100206000396000f3fe608060405234801561001057600080fd5b50600436106100365760003560e01c80632ddea1791461003b5780637975cd031461006e575b600080fd5b610055600480360381019061005091906104ff565b6100a2565b60405161006594939291906108d4565b60405180910390f35b61008860048036038101906100839190610c21565b6100c0565b604051610099959493929190610edf565b60405180910390f35b60006060806060878787879350935093509350945094509450949050565b6000606080600060608989898989945094509450945094509550955095509550959050565b6000604051905090565b600080fd5b600080fd5b6000819050919050565b61010c816100f9565b811461011757600080fd5b50565b60008135905061012981610103565b92915050565b600080fd5b600080fd5b6000601f19601f8301169050919050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052604160045260246000fd5b61018282610139565b810181811067ffffffffffffffff821117156101a1576101a061014a565b5b80604052505050565b60006101b46100e5565b90506101c08282610179565b919050565b600067ffffffffffffffff8211156101e0576101df61014a565b5b6101e982610139565b9050602081019050919050565b82818337600083830152505050565b6000610218610213846101c5565b6101aa565b90508281526020810184848401111561023457610233610134565b5b61023f8482856101f6565b509392505050565b600082601f83011261025c5761025b61012f565b5b813561026c848260208601610205565b91505092915050565b600067ffffffffffffffff8211156102905761028f61014a565b5b602082029050602081019050919050565b600080fd5b60006102b96102b484610275565b6101aa565b905080838252602082019050602084028301858111156102dc576102db6102a1565b5b835b8181101561030557806102f1888261011a565b8452602084019350506020810190506102de565b5050509392505050565b600082601f8301126103245761032361012f565b5b81356103348482602086016102a6565b91505092915050565b600067ffffffffffffffff8211156103585761035761014a565b5b602082029050602081019050919050565b600067ffffffffffffffff8211156103845761038361014a565b5b602082029050602081019050919050565b60006103a86103a384610369565b6101aa565b905080838252602082019050602084028301858111156103cb576103ca6102a1565b5b835b8181101561041257803567ffffffffffffffff8111156103f0576103ef61012f565b5b8086016103fd8982610247565b855260208501945050506020810190506103cd565b5050509392505050565b600082601f8301126104315761043061012f565b5b8135610441848260208601610395565b91505092915050565b600061045d6104588461033d565b6101aa565b905080838252602082019050602084028301858111156104805761047f6102a1565b5b835b818110156104c757803567ffffffffffffffff8111156104a5576104a461012f565b5b8086016104b2898261041c565b85526020850194505050602081019050610482565b5050509392505050565b600082601f8301126104e6576104e561012f565b5b81356104f684826020860161044a565b91505092915050565b60008060008060808587031215610519576105186100ef565b5b60006105278782880161011a565b945050602085013567ffffffffffffffff811115610548576105476100f4565b5b61055487828801610247565b935050604085013567ffffffffffffffff811115610575576105746100f4565b5b6105818782880161030f565b925050606085013567ffffffffffffffff8111156105a2576105a16100f4565b5b6105ae878288016104d1565b91505092959194509250565b6105c3816100f9565b82525050565b600081519050919050565b600082825260208201905092915050565b60005b838110156106035780820151818401526020810190506105e8565b60008484015250505050565b600061061a826105c9565b61062481856105d4565b93506106348185602086016105e5565b61063d81610139565b840191505092915050565b600081519050919050565b600082825260208201905092915050565b6000819050602082019050919050565b61067d816100f9565b82525050565b600061068f8383610674565b60208301905092915050565b6000602082019050919050565b60006106b382610648565b6106bd8185610653565b93506106c883610664565b8060005b838110156106f95781516106e08882610683565b97506106eb8361069b565b9250506001810190506106cc565b5085935050505092915050565b600081519050919050565b600082825260208201905092915050565b6000819050602082019050919050565b600081519050919050565b600082825260208201905092915050565b6000819050602082019050919050565b600082825260208201905092915050565b600061077a826105c9565b610784818561075e565b93506107948185602086016105e5565b61079d81610139565b840191505092915050565b60006107b4838361076f565b905092915050565b6000602082019050919050565b60006107d482610732565b6107de818561073d565b9350836020820285016107f08561074e565b8060005b8581101561082c578484038952815161080d85826107a8565b9450610818836107bc565b925060208a019950506001810190506107f4565b50829750879550505050505092915050565b600061084a83836107c9565b905092915050565b6000602082019050919050565b600061086a82610706565b6108748185610711565b93508360208202850161088685610722565b8060005b858110156108c257848403895281516108a3858261083e565b94506108ae83610852565b925060208a0199505060018101905061088a565b50829750879550505050505092915050565b60006080820190506108e960008301876105ba565b81810360208301526108fb818661060f565b9050818103604083015261090f81856106a8565b90508181036060830152610923818461085f565b905095945050505050565b60008115159050919050565b6109438161092e565b811461094e57600080fd5b50565b6000813590506109608161093a565b92915050565b600067ffffffffffffffff8211156109815761098061014a565b5b602082029050602081019050919050565b60006109a56109a084610966565b6101aa565b905080838252602082019050602084028301858111156109c8576109c76102a1565b5b835b818110156109f157806109dd8882610951565b8452602084019350506020810190506109ca565b5050509392505050565b600082601f830112610a1057610a0f61012f565b5b8135610a20848260208601610992565b91505092915050565b600067ffffffffffffffff821115610a4457610a4361014a565b5b610a4d82610139565b9050602081019050919050565b6000610a6d610a6884610a29565b6101aa565b905082815260208101848484011115610a8957610a88610134565b5b610a948482856101f6565b509392505050565b600082601f830112610ab157610ab061012f565b5b8135610ac1848260208601610a5a565b91505092915050565b6000819050919050565b610add81610aca565b8114610ae857600080fd5b50565b600081359050610afa81610ad4565b92915050565b600067ffffffffffffffff821115610b1b57610b1a61014a565b5b602082029050602081019050919050565b600073ffffffffffffffffffffffffffffffffffffffff82169050919050565b6000610b5782610b2c565b9050919050565b610b6781610b4c565b8114610b7257600080fd5b50565b600081359050610b8481610b5e565b92915050565b6000610b9d610b9884610b00565b6101aa565b90508083825260208201905060208402830185811115610bc057610bbf6102a1565b5b835b81811015610be95780610bd58882610b75565b845260208401935050602081019050610bc2565b5050509392505050565b600082601f830112610c0857610c0761012f565b5b8135610c18848260208601610b8a565b91505092915050565b600080600080600060a08688031215610c3d57610c3c6100ef565b5b6000610c4b88828901610951565b955050602086013567ffffffffffffffff811115610c6c57610c6b6100f4565b5b610c78888289016109fb565b945050604086013567ffffffffffffffff811115610c9957610c986100f4565b5b610ca588828901610a9c565b9350506060610cb688828901610aeb565b925050608086013567ffffffffffffffff811115610cd757610cd66100f4565b5b610ce388828901610bf3565b9150509295509295909350565b610cf98161092e565b82525050565b600081519050919050565b600082825260208201905092915050565b6000819050602082019050919050565b610d348161092e565b82525050565b6000610d468383610d2b565b60208301905092915050565b6000602082019050919050565b6000610d6a82610cff565b610d748185610d0a565b9350610d7f83610d1b565b8060005b83811015610db0578151610d978882610d3a565b9750610da283610d52565b925050600181019050610d83565b5085935050505092915050565b600081519050919050565b600082825260208201905092915050565b6000610de482610dbd565b610dee8185610dc8565b9350610dfe8185602086016105e5565b610e0781610139565b840191505092915050565b610e1b81610aca565b82525050565b600081519050919050565b600082825260208201905092915050565b6000819050602082019050919050565b610e5681610b4c565b82525050565b6000610e688383610e4d565b60208301905092915050565b6000602082019050919050565b6000610e8c82610e21565b610e968185610e2c565b9350610ea183610e3d565b8060005b83811015610ed2578151610eb98882610e5c565b9750610ec483610e74565b925050600181019050610ea5565b5085935050505092915050565b600060a082019050610ef46000830188610cf0565b8181036020830152610f068187610d5f565b90508181036040830152610f1a8186610dd9565b9050610f296060830185610e12565b8181036080830152610f3b8184610e81565b9050969550505050505056fea2646970667358221220ad6726e19142fa8c7413c226a2389fe9d58d182b0960bd033d69d4245125c17364736f6c63430008110033";

        private static string complexContractAddress;
        EOAWallet wallet1 = new EOAWallet("0xabc0000000000000000000000000000000000000000000000000000000000001");
        EOAWallet wallet2 = new EOAWallet("0xabc0000000000000000000000000000000000000000000000000000000000002");
        EOAWallet wallet3 = new EOAWallet("0xabc0000000000000000000000000000000000000000000000000000000000003");
        SequenceEthClient client = new SequenceEthClient("http://localhost:8545/");

        [SetUp]
        public async Task DeployComplexContract()
        {
            try
            {
                ContractDeploymentResult receipt = await ContractDeployer.Deploy(client, wallet1, complexContractBytecode);
                complexContractAddress = receipt.Receipt.contractAddress;
                Assert.IsNotEmpty(complexContractAddress);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [Test]
        public async Task TestComplexContract()
        {
            BigInteger expectedNumber = 5;
            string expectedWord = "SDK by Horizon";
            BigInteger[] expectedNumbers = new BigInteger[] { 1, 2, 3, 4, 5 };
            string[][] expectedDoubleNestedWords = new string[][]
            {
                new string[] { },
                new string[]
                    { "SDK by Horizon", "", "Hello World!", DecodeABITests.longMultiLineString, "", "SDK by Horizon" },
                new string[] { "", "", "" },
                new string[] { "" }
            };
            bool expectedBoolean = true;
            bool[] expectedBools = new bool[] { true, true, false, true };
            byte[] expectedByteArray = Encoding.UTF8.GetBytes("SDK by Horizon");
            byte[] moreBytes = Encoding.UTF8.GetBytes("Hello World!");
            FixedByte expectedMoreBytes = new FixedByte(32, moreBytes);
            Address[] expectedAddresses = new Address[]
                { wallet1.GetAddress(), wallet2.GetAddress(), wallet3.GetAddress() };

            Contract complexContract = new Contract(complexContractAddress, complexContractAbi);
            object[] resultPart1 = await complexContract.QueryContract<object[]>("testPart1", expectedNumber, expectedWord,
                expectedNumbers,
                expectedDoubleNestedWords)(client);
            object[] resultPart2 = await complexContract.QueryContract<object[]>("testPart2", expectedBoolean,
                expectedBools, expectedByteArray, expectedMoreBytes, expectedAddresses).SendQuery(client);
            
            Assert.AreEqual(expectedNumber, (BigInteger)resultPart1[0]);
            Assert.AreEqual(expectedWord, (string)resultPart1[1]);
            CollectionAssert.AreEqual(expectedNumbers, resultPart1[2].ConvertToTArray<BigInteger, object>());
            if (resultPart1[3] is Array array)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    CollectionAssert.AreEqual(expectedDoubleNestedWords[i], array.GetValue(i).ConvertToTArray<string, object>());
                }
            }
            else
            {
                Assert.Fail($"Expected an array but got: {resultPart1[3]}");
            }
            Assert.AreEqual(expectedBoolean, (bool)resultPart2[0]);
            CollectionAssert.AreEqual(expectedBools, resultPart2[1].ConvertToTArray<bool, object>());
            CollectionAssert.AreEqual(expectedByteArray, (byte[])resultPart2[2]);
            CollectionAssert.AreEqual(expectedMoreBytes.Data, (byte[])resultPart2[3]);
            CollectionAssert.AreEqual(expectedAddresses, resultPart2[4].ConvertToTArray<Address, object>());
        }

        [Test]
        public void TestInvalidRegex_noABI()
        {
            Contract complexContract = new Contract(complexContractAddress);

            try
            {
                complexContract.QueryContract<string>("functionName");
                Assert.Fail("Expected exception but none was thrown");
            }
            catch (ArgumentException e)
            {

            }
            catch (Exception e)
            {
                Assert.Fail("Expected argument exception");
            }

            try
            {
                complexContract.QueryContract<string>("functionName(");
                Assert.Fail("Expected exception but none was thrown");
            }
            catch (ArgumentException e)
            {

            }
            catch (Exception e)
            {
                Assert.Fail("Expected argument exception");
            }

            try
            {
                complexContract.QueryContract<string>("functionName)");
                Assert.Fail("Expected exception but none was thrown");
            }
            catch (ArgumentException e)
            {

            }
            catch (Exception e)
            {
                Assert.Fail("Expected argument exception");
            }

            try
            {
                complexContract.AssembleCallData("functionName(uint banana)", 1);
            }
            catch (Exception e)
            {
                Assert.Fail("No exception expected");
            }
            
            
            try
            {
                complexContract.CallFunction("functionName(uint, uint)", 1, 1);
            }
            catch (Exception e)
            {
                Assert.Fail("No exception expected");
            }
        }

        [Test]
        public void TestInvalidRegex_withABI()
        {
            Contract complexContract = new Contract(complexContractAddress, complexContractAbi);

            try
            {
                complexContract.QueryContract<string>("functionName()");
                Assert.Fail("Expected exception but none was thrown");
            }
            catch (ArgumentException e)
            {

            }
            catch (Exception e)
            {
                Assert.Fail("Expected argument exception");
            }

            try
            {
                complexContract.AssembleCallData("functionName(uint banana)");
                Assert.Fail("Expected exception but none was thrown");
            }
            catch (ArgumentException e)
            {

            }
            catch (Exception e)
            {
                Assert.Fail("Expected argument exception");
            }
            
            
            try
            {
                complexContract.CallFunction("functionName*");
                Assert.Fail("Expected exception but none was thrown");
            }
            catch (ArgumentException e)
            {

            }
            catch (Exception e)
            {
                Assert.Fail("Expected argument exception");
            }
        }
    }
}
