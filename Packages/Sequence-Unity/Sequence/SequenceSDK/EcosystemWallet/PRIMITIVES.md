# Sequence Ecosystem Wallets - Primitives

The goal of this Technical Design Document (TDD) is to document the architecture for each primitive.
The integration of v3 primitives in C# is fully encapsulated from Unity and aims to be an object-oriented library for 
Sequence's v3 wallets.

## Models

---

### Topology

A tree like structure is shared between the Config, Session, and Recover primitives. We call this type a `ITopology` 
which can either be represented as one of the following types:

- `IBranch` references an array of `ITopology` as it's children.
- `INode` references a hashed or encoded leaf. Compared to a leaf, a node does not need to be hashed separately.
- `ILeaf` represents the end of a branch and the type in the topology holding state information - such as signer addresses, configuration values or signatures.

Furthermore, the interface forces implementing classes to convert itself as one of the following types, defining
functions such as `ToJsonObject`, `Encode`, and `EncodeForHash`

- **Bytes:** Encode each topology, node, branch and leaf to a byte array.
Each encoded leaf and branch starts with one byte in the beginning as a 'flag' defining the type of leaf. 
This is required for when we want to decode the encoded data back into it's original topology.
Implementing topologies have their own decoding function to decode themself from a json string or byte array.
Bytes are represented as hexadecimal strings for API calls.
- **Hash:** Leafs and branches have a separate encoding function for hashing purposes called `EncodeForHash`.
Encoding a topology for hashing purposes does not require a decode-able flag at the beginning, resulting in a different hash. 
- **Json:** A topology must parse itself to a json-readable string. This is used with the integration tests when decoding a encoded topology. 
TODO: Refactor primitives to have their own JsonConvertors.

We will cover the different integrations of `ITopology` in the the next section.

## Primitives

---

### Payload

TBD

### Config Topology

TBD

### Session Topology

TBD

### Recovery

TBD

### Passkeys

TBD

### Address

## Testing Infrastructure

---

### Integration Tests

v3 Primitives can be tested using `forge` and the smart contract tests from the `0xsequence/wallet-contracts-v3` repository.
A guide on how to run integrations tests and verify the primitives [can be found here in the sequence-unity repository.](https://github.com/0xsequence/sequence-unity/blob/Feature/primitives-config/Assets/SequenceSDK/EcosystemWallet/IntegrationTests/README.md)
Tests include the following, and several more:

Integration tests are executed through API calls coming from the forge tests.
A test call is defined with a `method` key a json object called `parameters`

- **payload_toPacked:** Decode a parented payload and encode its payload value.
- **config_imageHash:** Parse a config from a json object and return the image hash for it.
- **session_encodeTopology:** Parse a session topology from a json object and return its encoded hex string.
- ...

### Unit Tests

Next to integration tests, we integrate a test for each primitive use case, such as encoding- or decoding configs, 
using Unity's test framework. You run unit tests in the Unity project of the `0xsequence/sequence-unity` repository. 
Then, open the `TestRunner` and search for `Ecosystem Wallet > UnitTests` Tests include the following, and several more:

- **Session Encoding:** Create a new session topology with a single identity signer and compare the encoded result.
- **Add Explicit Session:** Parse a session topology from a json string and add one explicit session leaf. 
- **Trim Recovery Leaves:** Removes a signer address from a given recovery topology.
- ...

## Dependencies

---

- Newtonsoft.Json
- Nethereum.ABI
- Nethereum.Hex
- Nethereum.Web3
