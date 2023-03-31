using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace SequenceSharp
{
    public class ERC20
    {
        private Web3 _web3 = null;
        private string _contractAddress = "";
        public Nethereum.Contracts.Contract contract { get; }

        public ERC20(Web3 web3, string contractAddress)
        {
            _web3 = web3;
            _contractAddress = contractAddress;
            contract = _web3.Eth.GetContract(abi, _contractAddress);
        }

        /// <summary>
        /// Returns the name of the ERC20.
        /// </summary>
        public Task<string> Name()
        {
            return contract.GetFunction("name").CallAsync<string>();
        }

        /// <summary>
        /// Returns the symbol of the ERC20, usually a shorter version of the name.
        /// </summary>
        public Task<string> Symbol()
        {
            return contract.GetFunction("symbol").CallAsync<string>();
        }

        /// <summary>
        /// Returns the number of decimals this ERC20 uses to get its user representation.
        /// Tokens usually opt for a value of 18, imitating the relationship between Ether and Wei.
        /// </summary>
        public Task<BigInteger> Decimals()
        {
            return contract.GetFunction("decimals").CallAsync<BigInteger>();
        }

        /// <summary>
        /// Returns the amount of this ERC20 in existence.
        /// </summary>
        public Task<BigInteger> TotalSupply()
        {
            return contract.GetFunction("totalSupply").CallAsync<BigInteger>();
        }

        /// <summary>
        /// Returns the amount of this ERC20 owned by an account.
        /// </summary>
        /// <param name="accountAddress"> Account address to read the balance from.</param>
        public Task<BigInteger> BalanceOf(string accountAddress)
        {
            return contract.GetFunction("balanceOf").CallAsync<BigInteger>(accountAddress);
        }

        /// <summary>
        /// Transfers some amount of this ERC20 to another address.
        /// </summary>
        public async Task<Nethereum.RPC.Eth.DTOs.TransactionReceipt> Transfer(string recipientAddress, BigInteger amount)
        {
            var address = await this._web3.GetAddress();
            UnityEngine.Debug.Log("Got ERC20 transfer call!");
            return await contract
                .GetFunction("transfer")
                .SendTransactionAndWaitForReceiptAsync(
                    address,
                    new HexBigInteger(BigInteger.Zero),
                    new HexBigInteger(BigInteger.Zero),
                    null,
                    recipientAddress,
                    amount
                );
        }

        /// <summary>
        /// Returns the remaining number of tokens that `spender` will be allowed to spend on behalf of `owner` through transferFrom. This is zero by default.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="spender"></param>
        /// <returns></returns>
        public Task<BigInteger> Allowance(string owner, string spender)
        {
            return contract.GetFunction("allowance").CallAsync<BigInteger>(owner, spender);
        }

        /// <summary>
        ///  Sets amount as the allowance of `spender` over the caller’s tokens.
        /// </summary>
        /// <param name="spenderAddress"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public async Task<Nethereum.RPC.Eth.DTOs.TransactionReceipt> Approve(string spenderAddress, string amount)
        {
            var address = await this._web3.GetAddress();
            return await contract
            .GetFunction("approve")
            .SendTransactionAndWaitForReceiptAsync(
                address,
                new HexBigInteger(BigInteger.Zero),
                new HexBigInteger(BigInteger.Zero),
                null,
                spenderAddress,
                amount
            );
        }

        /// <summary>
        /// Moves `amount` tokens from `from` to `to` using the allowance mechanism. amount is then deducted from the caller’s allowance.
        /// </summary>
        /// <param name="senderAddress"></param>
        /// <param name="recipientAddress"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public async Task<Nethereum.RPC.Eth.DTOs.TransactionReceipt> TransferFrom(
            string senderAddress,
            string recipientAddress,
            string amount
        )
        {
            var address = await this._web3.GetAddress();
            return await contract
               .GetFunction("transferFrom")
               .SendTransactionAndWaitForReceiptAsync(
                   address,
                   new HexBigInteger(BigInteger.Zero),
                   new HexBigInteger(BigInteger.Zero),
                   null,
                   senderAddress,
                   recipientAddress,
                   amount
               );
        }

        private static readonly string abi =
            @"
        [
          {
            ""inputs"": [],
            ""name"": ""name"",
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
            ""name"": ""symbol"",
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
            ""name"": ""decimals"",
            ""outputs"": [
              {
                ""internalType"": ""uint8"",
                ""name"": """",
                ""type"": ""uint8""
              }
            ],
            ""stateMutability"": ""view"",
            ""type"": ""function""
          },
          {
            ""inputs"": [],
            ""name"": ""totalSupply"",
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
                ""internalType"": ""address"",
                ""name"": ""account"",
                ""type"": ""address""
              }
            ],
            ""name"": ""balanceOf"",
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
                ""internalType"": ""address"",
                ""name"": ""recipient"",
                ""type"": ""address""
              },
              {
                ""internalType"": ""uint256"",
                ""name"": ""amount"",
                ""type"": ""uint256""
              }
            ],
            ""name"": ""transfer"",
            ""outputs"": [
              {
                ""internalType"": ""bool"",
                ""name"": """",
                ""type"": ""bool""
              }
            ],
            ""stateMutability"": ""nonpayable"",
            ""type"": ""function""
          },
          {
            ""inputs"": [
              {
                ""internalType"": ""address"",
                ""name"": ""sender"",
                ""type"": ""address""
              },
              {
                ""internalType"": ""address"",
                ""name"": ""recipient"",
                ""type"": ""address""
              },
              {
                ""internalType"": ""uint256"",
                ""name"": ""amount"",
                ""type"": ""uint256""
              }
            ],
            ""name"": ""transferFrom"",
            ""outputs"": [
              {
                ""internalType"": ""bool"",
                ""name"": """",
                ""type"": ""bool""
              }
            ],
            ""stateMutability"": ""nonpayable"",
            ""type"": ""function""
          },
          {
            ""inputs"": [
              {
                ""internalType"": ""address"",
                ""name"": ""spender"",
                ""type"": ""address""
              },
              {
                ""internalType"": ""uint256"",
                ""name"": ""amount"",
                ""type"": ""uint256""
              }
            ],
            ""name"": ""approve"",
            ""outputs"": [
              {
                ""internalType"": ""bool"",
                ""name"": """",
                ""type"": ""bool""
              }
            ],
            ""stateMutability"": ""nonpayable"",
            ""type"": ""function""
          },
          {
            ""inputs"": [
              {
                ""internalType"": ""address"",
                ""name"": ""owner"",
                ""type"": ""address""
              },
              {
                ""internalType"": ""address"",
                ""name"": ""spender"",
                ""type"": ""address""
              }
            ],
            ""name"": ""allowance"",
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
            ""anonymous"": false,
            ""inputs"": [
              {
                ""indexed"": true,
                ""internalType"": ""address"",
                ""name"": ""from"",
                ""type"": ""address""
              },
              {
                ""indexed"": true,
                ""internalType"": ""address"",
                ""name"": ""to"",
                ""type"": ""address""
              },
              {
                ""indexed"": false,
                ""internalType"": ""uint256"",
                ""name"": ""value"",
                ""type"": ""uint256""
              }
            ],
            ""name"": ""Transfer"",
            ""type"": ""event""
          },
          {
            ""anonymous"": false,
            ""inputs"": [
              {
                ""indexed"": true,
                ""internalType"": ""address"",
                ""name"": ""owner"",
                ""type"": ""address""
              },
              {
                ""indexed"": true,
                ""internalType"": ""address"",
                ""name"": ""spender"",
                ""type"": ""address""
              },
              {
                ""indexed"": false,
                ""internalType"": ""uint256"",
                ""name"": ""value"",
                ""type"": ""uint256""
              }
            ],
            ""name"": ""Approval"",
            ""type"": ""event""
          }
        ]";
    }
}
