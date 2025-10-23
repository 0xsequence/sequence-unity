# Sequence Ecosystem Wallets - Transactions

Transactions are encoded and signed natively in this SDK and send to our relayer.

## Implicit

Implicit transactions require a contract that supports them. We make an eth_call to the contract to validate
if a certain signer is allowed to perform an implicit transaction for a specified call.

## Explicit

Explicit sessions require a signer 

## Key Machine

There's a 'KeyMachineApi' class used to request wallet data such as the current image hash, config updates 
or the config- and session topologies from an image hash.

## Envelope

We pack together the calls with the parent address. Then when we sign calls, we include them in a 'signed envelope'
and encode that together to send the transactions to the relayer.

## Dependencies

---

- Newtonsoft.Json
- Nethereum.ABI
- Nethereum.Hex
- Nethereum.Web3