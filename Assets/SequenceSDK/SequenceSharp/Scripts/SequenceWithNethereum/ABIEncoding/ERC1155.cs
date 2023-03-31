using System.Numerics;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;


namespace SequenceSharp
{
    public class ERC1155
    {
        private Web3 _web3 = null;
        private string _contractAddress = "";
        public Nethereum.Contracts.Contract contract { get; }

        public ERC1155(Web3 web3, string contractAddress)
        {
            _web3 = web3;
            _contractAddress = contractAddress;
            contract = _web3.Eth.GetContract(abi, _contractAddress);
        }

        /// <summary>
        /// Returns the metadata URI for token `id`
        /// </summary>
        /// <param name="id">The Token ID to fetch the URI for.</param>
        /// <returns>A URI - can be IPFS, a URL, a raw svg, etc.</returns>
        public Task<string> URI(BigInteger id)
        {
            return contract.GetFunction("uri").CallAsync<string>(id);
        }

        /// <summary>
        /// Returns the amount of tokens of Token type `id` owned by `account`
        /// </summary>
        /// <param name="account">The address to query. Cannot be the zero address.</param>
        /// <param name="id">The Token ID to query.</param>
        /// <returns></returns>
        public Task<BigInteger> BalanceOf(BigInteger id, string account)
        {
            return contract.GetFunction("balanceOf").CallAsync<BigInteger>(account, id);
        }

        /// <summary>
        /// Batched version of BalanceOf. `accounts` and `ids` must have same length.
        /// </summary>
        /// <param name="accounts">A list of accounts to fetch balances for. Must have same length as ids.</param>
        /// <param name="ids">A list of IDs to fetch for each given account. Must have same length as accounts</param>
        /// <returns>A balance for each account.</returns>
        public Task<List<BigInteger>> BalanceOfBatch(List<string> accounts, List<BigInteger> ids)
        {
            return contract
                .GetFunction("balanceOfBatch")
                .CallAsync<List<BigInteger>>(accounts, ids);
        }

        /// <summary>
        /// Grants or revokes permission to operator to transfer the caller’s tokens, according to `approved`
        /// </summary>
        /// <param name="operatorAddress"></param>
        /// <param name="approved"></param>
        /// <returns></returns>
        public async Task<Nethereum.RPC.Eth.DTOs.TransactionReceipt> SetApprovalForAll(string operatorAddress, bool approved)
        {
            var address = await this._web3.GetAddress();
            return await contract
                 .GetFunction("setApprovalForAll")
                 .SendTransactionAndWaitForReceiptAsync(
                     address,
                     new HexBigInteger(BigInteger.Zero),
                     new HexBigInteger(BigInteger.Zero),
                     null,
                     operatorAddress,
                     approved
                 );
        }

        /// <summary>
        /// Returns true if `operator` is approved to transfer account's tokens.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="operatorAddress"></param>
        /// <returns></returns>
        public Task<bool> IsApprovedForAll(string account, string operatorAddress)
        {
            return contract
                .GetFunction("isApprovedForAll")
                .CallAsync<bool>(
                    account,
                    operatorAddress
                 );
        }

        /// <summary>
        /// Transfers `amount` tokens of token type `id` from `from` to `to`.
        /// </summary>
        /// <param name="from">must have a balance of tokens of type `id` of at least `amount`</param>
        /// <param name="to">cannot be the zero address</param>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<Nethereum.RPC.Eth.DTOs.TransactionReceipt> SafeTransferFrom(
            string from,
            string to,
            BigInteger id,
            BigInteger amount,
            Byte[] data = null
        )
        {
            var address = await this._web3.GetAddress();
            return await contract
                .GetFunction("safeTransferFrom")
                .SendTransactionAndWaitForReceiptAsync(
                    address,
                    new HexBigInteger(BigInteger.Zero),
                    new HexBigInteger(BigInteger.Zero),
                    null,
                    from,
                    to,
                    id,
                    amount,
                    data == null ? new Byte[] { } : data
                );
        }

        /// <summary>
        /// Batched version of safeTransferFrom, `ids` and `amounts` must have the same length.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="ids"></param>
        /// <param name="amounts"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<Nethereum.RPC.Eth.DTOs.TransactionReceipt> SafeBatchTransferFrom(
            string from,
            string to,
            List<BigInteger> ids,
            List<BigInteger> amounts,
            Byte[] data
        )
        {
            var address = await this._web3.GetAddress();
            return await contract
                .GetFunction("safeBatchTransferFrom")
                .SendTransactionAndWaitForReceiptAsync(
                    address,
                    new HexBigInteger(BigInteger.Zero),
                    new HexBigInteger(BigInteger.Zero),
                    null,
                    from,
                    to,
                    ids,
                    amounts,
                    data == null ? new Byte[] { } : data
                );
        }

        private static readonly string abi =
            @"
            [
                {
                    ""anonymous"": false,
                    ""inputs"": [
                    {
                        ""indexed"": true,
                        ""internalType"": ""address"",
                        ""name"": ""account"",
                        ""type"": ""address""
                    },
                    {
                        ""indexed"": true,
                        ""internalType"": ""address"",
                        ""name"": ""operator"",
                        ""type"": ""address""
                    },
                    {
                        ""indexed"": false,
                        ""internalType"": ""bool"",
                        ""name"": ""approved"",
                        ""type"": ""bool""
                    }
                    ],
                    ""name"": ""ApprovalForAll"",
                    ""type"": ""event""
                },
                {
                    ""anonymous"": false,
                    ""inputs"": [
                    {
                        ""indexed"": true,
                        ""internalType"": ""address"",
                        ""name"": ""operator"",
                        ""type"": ""address""
                    },
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
                        ""internalType"": ""uint256[]"",
                        ""name"": ""ids"",
                        ""type"": ""uint256[]""
                    },
                    {
                        ""indexed"": false,
                        ""internalType"": ""uint256[]"",
                        ""name"": ""values"",
                        ""type"": ""uint256[]""
                    }
                    ],
                    ""name"": ""TransferBatch"",
                    ""type"": ""event""
                },
                {
                    ""anonymous"": false,
                    ""inputs"": [
                    {
                        ""indexed"": true,
                        ""internalType"": ""address"",
                        ""name"": ""operator"",
                        ""type"": ""address""
                    },
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
                        ""name"": ""id"",
                        ""type"": ""uint256""
                    },
                    {
                        ""indexed"": false,
                        ""internalType"": ""uint256"",
                        ""name"": ""value"",
                        ""type"": ""uint256""
                    }
                    ],
                    ""name"": ""TransferSingle"",
                    ""type"": ""event""
                },
                {
                    ""anonymous"": false,
                    ""inputs"": [
                    {
                        ""indexed"": false,
                        ""internalType"": ""string"",
                        ""name"": ""value"",
                        ""type"": ""string""
                    },
                    {
                        ""indexed"": true,
                        ""internalType"": ""uint256"",
                        ""name"": ""id"",
                        ""type"": ""uint256""
                    }
                    ],
                    ""name"": ""URI"",
                    ""type"": ""event""
                },
                {
                    ""inputs"": [
                    {
                        ""internalType"": ""address"",
                        ""name"": ""account"",
                        ""type"": ""address""
                    },
                    {
                        ""internalType"": ""uint256"",
                        ""name"": ""id"",
                        ""type"": ""uint256""
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
                        ""internalType"": ""address[]"",
                        ""name"": ""accounts"",
                        ""type"": ""address[]""
                    },
                    {
                        ""internalType"": ""uint256[]"",
                        ""name"": ""ids"",
                        ""type"": ""uint256[]""
                    }
                    ],
                    ""name"": ""balanceOfBatch"",
                    ""outputs"": [
                    {
                        ""internalType"": ""uint256[]"",
                        ""name"": """",
                        ""type"": ""uint256[]""
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
                    },
                    {
                        ""internalType"": ""address"",
                        ""name"": ""operator"",
                        ""type"": ""address""
                    }
                    ],
                    ""name"": ""isApprovedForAll"",
                    ""outputs"": [
                    {
                        ""internalType"": ""bool"",
                        ""name"": """",
                        ""type"": ""bool""
                    }
                    ],
                    ""stateMutability"": ""view"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [
                    {
                        ""internalType"": ""address"",
                        ""name"": ""from"",
                        ""type"": ""address""
                    },
                    {
                        ""internalType"": ""address"",
                        ""name"": ""to"",
                        ""type"": ""address""
                    },
                    {
                        ""internalType"": ""uint256[]"",
                        ""name"": ""ids"",
                        ""type"": ""uint256[]""
                    },
                    {
                        ""internalType"": ""uint256[]"",
                        ""name"": ""amounts"",
                        ""type"": ""uint256[]""
                    },
                    {
                        ""internalType"": ""bytes"",
                        ""name"": ""data"",
                        ""type"": ""bytes""
                    }
                    ],
                    ""name"": ""safeBatchTransferFrom"",
                    ""outputs"": [],
                    ""stateMutability"": ""nonpayable"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [
                    {
                        ""internalType"": ""address"",
                        ""name"": ""from"",
                        ""type"": ""address""
                    },
                    {
                        ""internalType"": ""address"",
                        ""name"": ""to"",
                        ""type"": ""address""
                    },
                    {
                        ""internalType"": ""uint256"",
                        ""name"": ""id"",
                        ""type"": ""uint256""
                    },
                    {
                        ""internalType"": ""uint256"",
                        ""name"": ""amount"",
                        ""type"": ""uint256""
                    },
                    {
                        ""internalType"": ""bytes"",
                        ""name"": ""data"",
                        ""type"": ""bytes""
                    }
                    ],
                    ""name"": ""safeTransferFrom"",
                    ""outputs"": [],
                    ""stateMutability"": ""nonpayable"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [
                    {
                        ""internalType"": ""address"",
                        ""name"": ""operator"",
                        ""type"": ""address""
                    },
                    {
                        ""internalType"": ""bool"",
                        ""name"": ""approved"",
                        ""type"": ""bool""
                    }
                    ],
                    ""name"": ""setApprovalForAll"",
                    ""outputs"": [],
                    ""stateMutability"": ""nonpayable"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [
                    {
                        ""internalType"": ""bytes4"",
                        ""name"": ""interfaceId"",
                        ""type"": ""bytes4""
                    }
                    ],
                    ""name"": ""supportsInterface"",
                    ""outputs"": [
                    {
                        ""internalType"": ""bool"",
                        ""name"": """",
                        ""type"": ""bool""
                    }
                    ],
                    ""stateMutability"": ""view"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [
                    {
                        ""internalType"": ""uint256"",
                        ""name"": ""id"",
                        ""type"": ""uint256""
                    }
                    ],
                    ""name"": ""uri"",
                    ""outputs"": [
                    {
                        ""internalType"": ""string"",
                        ""name"": """",
                        ""type"": ""string""
                    }
                    ],
                    ""stateMutability"": ""view"",
                    ""type"": ""function""
                }
            ]";
    }
}
