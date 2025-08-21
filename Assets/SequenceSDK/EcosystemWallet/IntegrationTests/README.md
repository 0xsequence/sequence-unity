These integration tests require a local instance of the server described (and implemented) [here](https://github.com/0xsequence/wallet-contracts-v3). Basically, how these tests work is that we expose a local server from Unity that gets hit with a variety of requests from the testing server (linked) running locally on the system; the Unity-exposed server should handle these requests appropriately in order to pass the tests. These tests are useful to confirm that the primitives are working properly. They will run indefinitely and hit the SDK with a variety of random inputs to see if it handles them correctly. Since they run indefinitely, it is best to only run these when interacting with the Sequence.EcosystemWallet.Primitives namespace.

To run, first navigate to the Unity Editor and from the top bar click `Sequence Dev > Start Wallet V3 Test Server`; this will expose the Unity local server to localhost:8080 on your system.

Next, to begin hitting the Unity-exposed server with requests, perform the following:
1. Clone https://github.com/0xsequence/wallet-contracts-v3
2. Navigate to the newly cloned directory
3. `pnpm install`
4. `cp .env.sample .env`
5. Modify `.env` such that it has a private key and points to your Unity server (change both ports to 8080) -> one-time setup complete
6. `forge test` to run the tests and begin hitting our Unity-exposed server with requests