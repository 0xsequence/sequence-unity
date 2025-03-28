# Testchain
This project contains a hardhat testchain which can be run with `yarn start:hardhat`.

In addition, we've included some smart contracts. These are compiled with Foundry/Forge.

## Foundry

**Foundry is a blazing fast, portable and modular toolkit for Ethereum application development written in Rust.**

Foundry consists of:

-   **Forge**: Ethereum testing framework (like Truffle, Hardhat and DappTools).
-   **Cast**: Swiss army knife for interacting with EVM smart contracts, sending transactions and getting chain data.
-   **Anvil**: Local Ethereum node, akin to Ganache, Hardhat Network.
-   **Chisel**: Fast, utilitarian, and verbose solidity REPL.

## Documentation

https://book.getfoundry.sh/

## Installation
https://book.getfoundry.sh/getting-started/installation

## Usage

### Build

From /testchain
```shell
$ forge build
```

This will dump compiled contracts into `/artifacts` 

### Installing/upgrading dependencies

This project has an unusual setup as it is nested in a much larger monorepo. In order to install or update Sequence dependancies please perform the following:

```shell
cd ..
forge install https://github.com/0xsequence/contracts-library.git --no-commit
```
Replace the url with another git url if using dependencies from elsewhere.

This will install at `../lib/`. We want to move here.

```shell
cd .. 
ls lib/ (find out the name of the folder to move, in this case contracts-library)
mv lib/contracts-library testchain/lib/
```

Update entry in .gitmodules

Update foundry.toml such that remappings are correct
