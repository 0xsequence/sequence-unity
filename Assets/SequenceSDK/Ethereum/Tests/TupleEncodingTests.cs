using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.Provider;
using Sequence.Utils;
using Sequence.Wallet;
using UnityEngine;

namespace Sequence.Ethereum.Tests
{
    public class TupleEncodingTests
    {
        [Test]
        public async Task TestWeCanEncodeUserContract()
        {
            string erc20Address = "0xd55328F24A19C978c79dB7eAAd126DFE842543ec";
            string abi =
                        "[\r\n   {\r\n      \"type\":\"function\",\r\n      \"name\":\"nonces\",\r\n      \"inputs\":[\r\n         {\r\n            \"name\":\"\",\r\n            \"type\":\"address\",\r\n            \"internalType\":\"address\"\r\n         }\r\n      ],\r\n      \"outputs\":[\r\n         {\r\n            \"name\":\"\",\r\n            \"type\":\"uint256\",\r\n            \"internalType\":\"uint256\"\r\n         }\r\n      ],\r\n      \"stateMutability\":\"view\"\r\n   },\r\n   {\r\n      \"inputs\":[\r\n         {\r\n            \"internalType\":\"struct Batcher.Validation[]\",\r\n            \"name\":\"validationCalls\",\r\n            \"type\":\"tuple[]\",\r\n            \"components\":[\r\n               {\r\n                  \"internalType\":\"address\",\r\n                  \"name\":\"target\",\r\n                  \"type\":\"address\"\r\n               },\r\n               {\r\n                  \"internalType\":\"bytes\",\r\n                  \"name\":\"callData\",\r\n                  \"type\":\"bytes\"\r\n               },\r\n               {\r\n                  \"internalType\":\"bytes\",\r\n                  \"name\":\"expectedResult\",\r\n                  \"type\":\"bytes\"\r\n               },\r\n               {\r\n                  \"internalType\":\"enum Batcher.DataType\",\r\n                  \"name\":\"dataType\",\r\n                  \"type\":\"uint8\"\r\n               },\r\n               {\r\n                  \"internalType\":\"enum Batcher.Condition\",\r\n                  \"name\":\"condition\",\r\n                  \"type\":\"uint8\"\r\n               }\r\n            ]\r\n         },\r\n         {\r\n            \"internalType\":\"struct Batcher.Call[]\",\r\n            \"name\":\"calls\",\r\n            \"type\":\"tuple[]\",\r\n            \"components\":[\r\n               {\r\n                  \"internalType\":\"address\",\r\n                  \"name\":\"target\",\r\n                  \"type\":\"address\"\r\n               },\r\n               {\r\n                  \"internalType\":\"bytes\",\r\n                  \"name\":\"callData\",\r\n                  \"type\":\"bytes\"\r\n               }\r\n            ]\r\n         },\r\n         {\r\n            \"internalType\":\"bytes\",\r\n            \"name\":\"signature\",\r\n            \"type\":\"bytes\"\r\n         }\r\n      ],\r\n      \"stateMutability\":\"nonpayable\",\r\n      \"type\":\"function\",\r\n      \"name\":\"batchUserOperation\"\r\n   }\r\n]";

            Contract contract = new Contract(erc20Address, abi);
            var test = contract.CallFunction("batchUserOperation", 
                new Tuple<Address, byte[], byte[], int, int>[] { new (new Address(erc20Address), "0x00".ToByteArray(), "0x00".ToByteArray(), 0, 0) }, 
                new Tuple<string, byte[]>[] { new (erc20Address, "0x00".ToByteArray())},
                "0x00".ToByteArray());
            Assert.Pass();
        }
        
        private string _abi = @"
[
    {
        ""inputs"": [],
        ""name"": ""getStoredAddress"",
        ""outputs"": [
            {
                ""internalType"": ""address"",
                ""name"": """",
                ""type"": ""address""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""name"": ""getStoredAddress2"",
        ""outputs"": [
            {
                ""internalType"": ""address"",
                ""name"": """",
                ""type"": ""address""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""name"": ""getStoredBytes"",
        ""outputs"": [
            {
                ""internalType"": ""bytes"",
                ""name"": """",
                ""type"": ""bytes""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""name"": ""getStoredInt"",
        ""outputs"": [
            {
                ""internalType"": ""int256"",
                ""name"": """",
                ""type"": ""int256""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""name"": ""getStoredString"",
        ""outputs"": [
            {
                ""internalType"": ""string"",
                ""name"": """",
                ""type"": ""string""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""name"": ""getStoredString2"",
        ""outputs"": [
            {
                ""internalType"": ""string"",
                ""name"": """",
                ""type"": ""string""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""name"": ""getStoredUint"",
        ""outputs"": [
            {
                ""internalType"": ""uint256"",
                ""name"": """",
                ""type"": ""uint256""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""name"": ""storedAddress"",
        ""outputs"": [
            {
                ""internalType"": ""address"",
                ""name"": """",
                ""type"": ""address""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""name"": ""storedAddress2"",
        ""outputs"": [
            {
                ""internalType"": ""address"",
                ""name"": """",
                ""type"": ""address""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""name"": ""storedBytes"",
        ""outputs"": [
            {
                ""internalType"": ""bytes"",
                ""name"": """",
                ""type"": ""bytes""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""name"": ""storedInt"",
        ""outputs"": [
            {
                ""internalType"": ""int256"",
                ""name"": """",
                ""type"": ""int256""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""name"": ""storedString"",
        ""outputs"": [
            {
                ""internalType"": ""string"",
                ""name"": """",
                ""type"": ""string""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""name"": ""storedString2"",
        ""outputs"": [
            {
                ""internalType"": ""string"",
                ""name"": """",
                ""type"": ""string""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""name"": ""storedUint"",
        ""outputs"": [
            {
                ""internalType"": ""uint256"",
                ""name"": """",
                ""type"": ""uint256""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [
            {
                ""components"": [
                    {
                        ""internalType"": ""address"",
                        ""name"": ""someAddress"",
                        ""type"": ""address""
                    },
                    {
                        ""internalType"": ""string"",
                        ""name"": ""first"",
                        ""type"": ""string""
                    }
                ],
                ""internalType"": ""struct TestContractWithTuples.SingleTuple"",
                ""name"": ""singleTuple"",
                ""type"": ""tuple""
            },
            {
                ""components"": [
                    {
                        ""internalType"": ""string"",
                        ""name"": ""second"",
                        ""type"": ""string""
                    },
                    {
                        ""internalType"": ""address"",
                        ""name"": ""another"",
                        ""type"": ""address""
                    },
                    {
                        ""internalType"": ""bytes"",
                        ""name"": ""data"",
                        ""type"": ""bytes""
                    },
                    {
                        ""internalType"": ""uint256"",
                        ""name"": ""number"",
                        ""type"": ""uint256""
                    }
                ],
                ""internalType"": ""struct TestContractWithTuples.TupleArray[]"",
                ""name"": ""tupleArray"",
                ""type"": ""tuple[]""
            },
            {
                ""internalType"": ""int256"",
                ""name"": ""integer"",
                ""type"": ""int256""
            }
        ],
        ""name"": ""testTuple"",
        ""outputs"": [],
        ""stateMutability"": ""nonpayable"",
        ""type"": ""function""
    }
]";

        private string _bytecode =
            "0x608060405234801561001057600080fd5b5061138c806100206000396000f3fe608060405234801561001057600080fd5b50600436106100f55760003560e01c8063933f417011610097578063bee7414c11610066578063bee7414c14610242578063dcb21d1214610260578063dee563551461027e578063fac8b3391461029c576100f5565b8063933f4170146101cc5780639e13b4c2146101e8578063b2df597814610206578063b94d0c7414610224576100f5565b80636e4d2a34116100d35780636e4d2a341461015457806389db8c78146101725780638f63640e146101905780639000b934146101ae576100f5565b806302dd56d0146100fa578063352d965e1461011857806356d61e9414610136575b600080fd5b6101026102ba565b60405161010f91906108ba565b60405180910390f35b610120610348565b60405161012d919061091d565b60405180910390f35b61013e61036e565b60405161014b91906108ba565b60405180910390f35b61015c6103fc565b6040516101699190610951565b60405180910390f35b61017a610402565b60405161018791906109c1565b60405180910390f35b610198610494565b6040516101a5919061091d565b60405180910390f35b6101b66104b8565b6040516101c3919061091d565b60405180910390f35b6101e660048036038101906101e19190610e67565b6104e1565b005b6101f0610634565b6040516101fd9190610f01565b60405180910390f35b61020e61063e565b60405161021b9190610951565b60405180910390f35b61022c610648565b604051610239919061091d565b60405180910390f35b61024a610672565b60405161025791906109c1565b60405180910390f35b610268610700565b60405161027591906108ba565b60405180910390f35b610286610792565b60405161029391906108ba565b60405180910390f35b6102a4610824565b6040516102b19190610f01565b60405180910390f35b600280546102c790610f4b565b80601f01602080910402602001604051908101604052809291908181526020018280546102f390610f4b565b80156103405780601f1061031557610100808354040283529160200191610340565b820191906000526020600020905b81548152906001019060200180831161032357829003601f168201915b505050505081565b600160009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1681565b6003805461037b90610f4b565b80601f01602080910402602001604051908101604052809291908181526020018280546103a790610f4b565b80156103f45780601f106103c9576101008083540402835291602001916103f4565b820191906000526020600020905b8154815290600101906020018083116103d757829003601f168201915b505050505081565b60045481565b60606006805461041190610f4b565b80601f016020809104026020016040519081016040528092919081815260200182805461043d90610f4b565b801561048a5780601f1061045f5761010080835404028352916020019161048a565b820191906000526020600020905b81548152906001019060200180831161046d57829003601f168201915b5050505050905090565b60008054906101000a900473ffffffffffffffffffffffffffffffffffffffff1681565b60008060009054906101000a900473ffffffffffffffffffffffffffffffffffffffff16905090565b82600001516000806101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff1602179055508260200151600290816105389190611128565b508060058190555060008251111561062f578160008151811061055e5761055d6111fa565b5b602002602001015160200151600160006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff160217905550816000815181106105be576105bd6111fa565b5b602002602001015160000151600390816105d89190611128565b50816000815181106105ed576105ec6111fa565b5b602002602001015160400151600690816106079190611284565b508160008151811061061c5761061b6111fa565b5b6020026020010151606001516004819055505b505050565b6000600554905090565b6000600454905090565b6000600160009054906101000a900473ffffffffffffffffffffffffffffffffffffffff16905090565b6006805461067f90610f4b565b80601f01602080910402602001604051908101604052809291908181526020018280546106ab90610f4b565b80156106f85780601f106106cd576101008083540402835291602001916106f8565b820191906000526020600020905b8154815290600101906020018083116106db57829003601f168201915b505050505081565b60606002805461070f90610f4b565b80601f016020809104026020016040519081016040528092919081815260200182805461073b90610f4b565b80156107885780601f1061075d57610100808354040283529160200191610788565b820191906000526020600020905b81548152906001019060200180831161076b57829003601f168201915b5050505050905090565b6060600380546107a190610f4b565b80601f01602080910402602001604051908101604052809291908181526020018280546107cd90610f4b565b801561081a5780601f106107ef5761010080835404028352916020019161081a565b820191906000526020600020905b8154815290600101906020018083116107fd57829003601f168201915b5050505050905090565b60055481565b600081519050919050565b600082825260208201905092915050565b60005b83811015610864578082015181840152602081019050610849565b60008484015250505050565b6000601f19601f8301169050919050565b600061088c8261082a565b6108968185610835565b93506108a6818560208601610846565b6108af81610870565b840191505092915050565b600060208201905081810360008301526108d48184610881565b905092915050565b600073ffffffffffffffffffffffffffffffffffffffff82169050919050565b6000610907826108dc565b9050919050565b610917816108fc565b82525050565b6000602082019050610932600083018461090e565b92915050565b6000819050919050565b61094b81610938565b82525050565b60006020820190506109666000830184610942565b92915050565b600081519050919050565b600082825260208201905092915050565b60006109938261096c565b61099d8185610977565b93506109ad818560208601610846565b6109b681610870565b840191505092915050565b600060208201905081810360008301526109db8184610988565b905092915050565b6000604051905090565b600080fd5b600080fd5b600080fd5b7f4e487b7100000000000000000000000000000000000000000000000000000000600052604160045260246000fd5b610a3482610870565b810181811067ffffffffffffffff82111715610a5357610a526109fc565b5b80604052505050565b6000610a666109e3565b9050610a728282610a2b565b919050565b600080fd5b610a85816108fc565b8114610a9057600080fd5b50565b600081359050610aa281610a7c565b92915050565b600080fd5b600080fd5b600067ffffffffffffffff821115610acd57610acc6109fc565b5b610ad682610870565b9050602081019050919050565b82818337600083830152505050565b6000610b05610b0084610ab2565b610a5c565b905082815260208101848484011115610b2157610b20610aad565b5b610b2c848285610ae3565b509392505050565b600082601f830112610b4957610b48610aa8565b5b8135610b59848260208601610af2565b91505092915050565b600060408284031215610b7857610b776109f7565b5b610b826040610a5c565b90506000610b9284828501610a93565b600083015250602082013567ffffffffffffffff811115610bb657610bb5610a77565b5b610bc284828501610b34565b60208301525092915050565b600067ffffffffffffffff821115610be957610be86109fc565b5b602082029050602081019050919050565b600080fd5b600067ffffffffffffffff821115610c1a57610c196109fc565b5b610c2382610870565b9050602081019050919050565b6000610c43610c3e84610bff565b610a5c565b905082815260208101848484011115610c5f57610c5e610aad565b5b610c6a848285610ae3565b509392505050565b600082601f830112610c8757610c86610aa8565b5b8135610c97848260208601610c30565b91505092915050565b610ca981610938565b8114610cb457600080fd5b50565b600081359050610cc681610ca0565b92915050565b600060808284031215610ce257610ce16109f7565b5b610cec6080610a5c565b9050600082013567ffffffffffffffff811115610d0c57610d0b610a77565b5b610d1884828501610b34565b6000830152506020610d2c84828501610a93565b602083015250604082013567ffffffffffffffff811115610d5057610d4f610a77565b5b610d5c84828501610c72565b6040830152506060610d7084828501610cb7565b60608301525092915050565b6000610d8f610d8a84610bce565b610a5c565b90508083825260208201905060208402830185811115610db257610db1610bfa565b5b835b81811015610df957803567ffffffffffffffff811115610dd757610dd6610aa8565b5b808601610de48982610ccc565b85526020850194505050602081019050610db4565b5050509392505050565b600082601f830112610e1857610e17610aa8565b5b8135610e28848260208601610d7c565b91505092915050565b6000819050919050565b610e4481610e31565b8114610e4f57600080fd5b50565b600081359050610e6181610e3b565b92915050565b600080600060608486031215610e8057610e7f6109ed565b5b600084013567ffffffffffffffff811115610e9e57610e9d6109f2565b5b610eaa86828701610b62565b935050602084013567ffffffffffffffff811115610ecb57610eca6109f2565b5b610ed786828701610e03565b9250506040610ee886828701610e52565b9150509250925092565b610efb81610e31565b82525050565b6000602082019050610f166000830184610ef2565b92915050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052602260045260246000fd5b60006002820490506001821680610f6357607f821691505b602082108103610f7657610f75610f1c565b5b50919050565b60008190508160005260206000209050919050565b60006020601f8301049050919050565b600082821b905092915050565b600060088302610fde7fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff82610fa1565b610fe88683610fa1565b95508019841693508086168417925050509392505050565b6000819050919050565b600061102561102061101b84610938565b611000565b610938565b9050919050565b6000819050919050565b61103f8361100a565b61105361104b8261102c565b848454610fae565b825550505050565b600090565b61106861105b565b611073818484611036565b505050565b5b818110156110975761108c600082611060565b600181019050611079565b5050565b601f8211156110dc576110ad81610f7c565b6110b684610f91565b810160208510156110c5578190505b6110d96110d185610f91565b830182611078565b50505b505050565b600082821c905092915050565b60006110ff600019846008026110e1565b1980831691505092915050565b600061111883836110ee565b9150826002028217905092915050565b6111318261082a565b67ffffffffffffffff81111561114a576111496109fc565b5b6111548254610f4b565b61115f82828561109b565b600060209050601f8311600181146111925760008415611180578287015190505b61118a858261110c565b8655506111f2565b601f1984166111a086610f7c565b60005b828110156111c8578489015182556001820191506020850194506020810190506111a3565b868310156111e557848901516111e1601f8916826110ee565b8355505b6001600288020188555050505b505050505050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052603260045260246000fd5b60008190508160005260206000209050919050565b601f82111561127f5761125081611229565b61125984610f91565b81016020851015611268578190505b61127c61127485610f91565b830182611078565b50505b505050565b61128d8261096c565b67ffffffffffffffff8111156112a6576112a56109fc565b5b6112b08254610f4b565b6112bb82828561123e565b600060209050601f8311600181146112ee57600084156112dc578287015190505b6112e6858261110c565b86555061134e565b601f1984166112fc86611229565b60005b82811015611324578489015182556001820191506020850194506020810190506112ff565b86831015611341578489015161133d601f8916826110ee565b8355505b6001600288020188555050505b50505050505056fea2646970667358221220e846d9f19222d8392ff9438032bc511c179256dd42635aa5f4018d402083ef4564736f6c63430008110033";

        [Test]
        public async Task TestTupleEncoding()
        {
            EOAWallet wallet = new EOAWallet("0xabc0000000000000000000000000000000000000000000000000000000000001");
            SequenceEthClient client = new SequenceEthClient("http://localhost:8545/");
            ContractDeploymentResult result = await ContractDeployer.Deploy(client, wallet, _bytecode);
            TransactionReceipt receipt = result.Receipt;
            string contractAddress = receipt.contractAddress;
            Assert.IsNotEmpty(contractAddress);
            Assert.AreEqual(contractAddress, result.DeployedContractAddress.Value);
            
            Contract contract = new Contract(contractAddress, _abi);
            Address someAddress = new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249");
            Address anotherAddress = new Address("0x1099542D7dFaF6757527146C0aB9E70A967f71C0");
            string first = "banana";
            string second = "split";
            byte[] data = "Testing the Sequence Unity SDK".ToByteArray();
            uint number = 123456789;
            int integer = 987654321;
            receipt = await contract.CallFunction("testTuple",
                (someAddress, first),
                new (string, Address, byte[], uint)[] { new (second, anotherAddress, data, number) },
                integer).SendTransactionMethodAndWaitForReceipt(wallet, client);

            Address storedAddress = await contract.QueryContract<Address>("getStoredAddress").Invoke(client);
            Assert.AreEqual(someAddress, storedAddress);
            Address storedAddress2 = await contract.QueryContract<Address>("getStoredAddress2").Invoke(client);
            Assert.AreEqual(anotherAddress, storedAddress2);
            byte[] storedBytes = await contract.QueryContract<byte[]>("getStoredBytes").Invoke(client);
            Assert.AreEqual(data, storedBytes);
            int storedInt = await contract.QueryContract<int>("getStoredInt").Invoke(client);
            Assert.AreEqual(integer, storedInt);
            string storedString = await contract.QueryContract<string>("getStoredString").Invoke(client);
            Assert.AreEqual(first, storedString);
            string storedString2 = await contract.QueryContract<string>("getStoredString2").Invoke(client);
            Assert.AreEqual(second, storedString2);
            uint storedUint = await contract.QueryContract<uint>("getStoredUint").Invoke(client);
            Assert.AreEqual(number, storedUint);
        }
    }
}