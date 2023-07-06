# Sequence Unity SDK
This project is a work in progress - a version 2 for the Sequence Unity SDK

## Requirements
Unity 2021.3.6f1

## Testing
The project makes use of Unity's test runner. You can open the test runner window in 
Unity by navigating to `Window > General > Test Runner`.

Many of the tests make use of a Hardhat-based testchain. This testchain can be found 
in the root folder of the project - in case you are experiencing issues with it.

Before running tests, boot up the test chain with `make start-testchain`. You may find 
that you need to stop (control + c) the testchain and restart it between some of the 
tests.

### Testing the test chain
Occasionally, it may be necessary to test the testchain to a) confirm it is 
giving the behaviours you expect and b) to use for comparison with our 
Unity tests. We can safely assume that ethers (which we use to test the 
testchain) works correctly. To test please use `make test-testchain`. Test output will be in 
chaintest.out and will also be printed to the terminal. If you need to end the test suite 
early, use `Control+C` and `make stop`.

### Troubleshooting
Do you have tests that are failing that you don't think should be or were previously passing and you haven't changed anything?
Here are a few things to try:
1. If you are or were using a debugger, disconnect the debugger from Unity and then reconnect
2. Restart the test chain `Control+C` and `make start-testchain`
3. Restart Unity
* Also note that since tests on the testchain are being run sequentially, if a prior test fails, it may not 
have unwound properly and may leave the next test in an unexpected state - causing it to fail.

## Component Overview
The SDK is broken into a number of components with different responsibilities. This section will give an overview of some of the most important components for users and their intended purposes.

### Client
IEthClient provides an interface for clients. Clients handle the connection to blockchain networks, making various RPC requests. Any time you wish to query the blockchain or submit a transaction, you will need a client.

### Wallet
EthWallet implements a standard EOA wallet. A wallet keeps track of its own private/public key pair and address and is responsible for providing its private key to the signer when signing transactions.

### Transaction
A transaction, as implemented in EthTransaction, contains all the data and parameters for an EVM transaction. The object is used for initiating its RLP encoding (transactions must be signed and RLP encoded when submitted).

### Contract
A contract is responsible for creating transactions (for method calls) and messages (for queries) agaisnt it. These transactions are later signed by the wallet + signer and submitted (along with query messages) using a client.

## Architecture Decision Records

Please add any ADRs below. In the future, it may be worthwhile to move these into 
separate files, but for now since there are few ADRs, the README should suffice. 
Please use [Michael Nygard's template for 
ADRs](https://github.com/joelparkerhenderson/architecture-decision-record/blob/main/templates/decision-record-template-by-michael-nygard/index.md)

### ADR 1 - sequence-unity
June 21, 2023 - author: Quinn Purdy

#### Status
This ADR document is being made retroactively after inheriting the project.

#### Context
Sequence Unity SDK v1 was made quickly as a proof of concept. The SDK relies on 
Nethereum; a library that is overly heavy-weight. The SDK also relies on the Vuplex 
webview unity package - this package is not free, leading to developer frustrations.

#### Decision
Modifying the existing v1 SDK was deemed to be unworthy undertaking. Building a new 
SDK from scratch was determined to be faster and easier.

#### Consequences
Iteration on SDK v2 during development will be significantly faster and lower risk 
than modifying the 
existing SDK the customers are currently using. However, this means that current 
customers using v1 of the SDK can expect limited support during the development of SDK 
v2 as v1 will be deprecated. 
