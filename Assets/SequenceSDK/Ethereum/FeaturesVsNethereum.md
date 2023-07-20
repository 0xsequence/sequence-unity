# Nethereum Features

## ABI
Encoding
Decoding
EIP 712 support with EIP 2612 extension

## Accounts
Load from keystore
Create account/wallet
Nonce service - both in-memory and network based
Transaction signing
Transaction sending
Managed accounts - private key in keystore
Hd wallet

## Besu
I'm not really sure what the intended use for this namespace is. It doesn't appear useful for our needs.

## BigInteger
A bunch of BigInteger extensions

## BlockchainProcessing
"Crawl" blocks and transactions and read logs
https://docs.nethereum.com/en/latest/nethereum-block-processing-detail/

## Contracts
Subscribe to contract events
Deploy smart contract
Calling/transacting with a smart contract
Gas estimation
Code generation
Querying previous contract states

## ENS
ENS related features

## EVM
EVM simulator - experimental

## Geth
Mining
Geth management API

## Gnosis Safe
Gnosis Safe related features

## GSN
GSN (Gas Station Network) related features

## Hex
A bunch of hexadecimal estensions

## JsonRpc
RPC Methods
Authentication settings
IpcClient
Web socket client

## Merkle Tree
Merkle Tree related features - e.g. build a merkle tree, proofs, Patricia Tries, etc.

## Metamask
Metamask related features

## Blazor
A .NET Wasm SPA light blockchain explorer and wallet

## Optimism
Bridge
Optimism related functionality - I'm not really sure why a separate module is needed for this

## Parity
Parity management API

## Quorum
Nethereum Quorum is the Nethereum extension to interact with [Quorum](https://github.com/jpmorganchase/quorum), the permissioned implementation of Ethereum supporting data privacy created by JP Morgan.

## RLP
Encoding
Decoding

## RSK
RSK related functionality - I'm not really sure the intended use case

## Signer
ECDSA signing
Recover key from signature
Calculate V R S
AWS Key signing
Azure Key signing
EIP 712 signing
Ledger Signing
Trezor Signing

## SIWE
Sign-In With Ethereum support

## ERC721
ERC721 convenience functions

## ERC20
ERC20 convenience functions

## Unity
Some Unity related functionality (mostly just using Unity Web Requests)
ERC1155 convenience functions
IPFS http service

## Util 
A bunch of miscellaneous data type extensions and other utility functions

## Web3
IPFS http service
Address checksums

# sequence-unity Features

## ABI
Encoding

## Accounts
Create account/wallet
Nonce service - network based only
Transaction signing
Transaction sending

## Contracts
Deploy smart contract
Calling/transacting with a smart contract
Gas estimation

## JsonRpc
RPC Methods

## RLP
Encoding
Decoding

## Signer
ECDSA signing
Recover key from signature
Calculate V R S

## ERC721
ERC721 convenience functions

## ERC20
ERC20 convenience functions

## ERC1155
ERC1155 convenience functions

## Web3
Address checksums

# sequence-unity Missing Features vs Nethereum

## ABI
Decoding
EIP 712 support with EIP 2612 extension

## Accounts
Load from keystore
Transaction signing - they differentiate between EIP1559 transactions and legacy transactions, we do not
In-memory nonce service (Note: initially, I had used an in-memory nonce service, but I have removed it since I found the UX could be challenging if the in-memory nonce service ever gets out of sync with the network)
Managed accounts - private key in keystore
Hd wallet

## BlockchainProcessing 
"Crawl" blocks and transactions and read logs
https://docs.nethereum.com/en/latest/nethereum-block-processing-detail/

## Contracts
Subscribe to contract events - note that Nethereum doesn't offer Unity support for this https://docs.nethereum.com/en/latest/unity3d-smartcontracts-getting-started/#logs-and-events
Code generation
Querying previous contract states

## ENS
ENS related features

## EVM
EVM simulator

## Geth
Mining
Geth management API

## Gnosis Safe
Gnosis Safe related features

## GSN
GSN (Gas Station Network) related features (fee-less transactions)

## JsonRpc
Authentication settings
Some of the JsonRpc methods have yet to be implemented in SequenceEthClient
IpcClient
Web socket client

## Merkle Tree
Merkle Tree related features - e.g. build a merkle tree, proofs, Patricia Tries, etc.

## Metamask
Metamask related features

## Blazor
A .NET Wasm SPA light blockchain explorer and wallet

## Optimism
Bridge
Optimism related functionality - I'm not really sure why a separate module is needed for this

## Parity
Parity management API

## Quorum
Nethereum Quorum is the Nethereum extension to interact with [Quorum](https://github.com/jpmorganchase/quorum), the permissioned implementation of Ethereum supporting data privacy created by JP Morgan.

## RSK
RSK related functionality - I'm not really sure the intended use case

## Signer
AWS Key signing
Azure Key signing
EIP 712 signing
Ledger Signing
Trezor Signing

## Web3
IPFS http service - note: Unity specific implementation in Nethereum.Unity
