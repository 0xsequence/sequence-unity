![header](https://github.com/user-attachments/assets/170d9147-3f2c-4b50-bccb-0e1d5a4b2aad)

# Sequence Unity SDK

The Sequence Unity Embedded Wallet SDK provides full Sequence Embedded Wallet and Indexer integration for your Unity Games, integrated with our own purpose-built for Unity SequenceEthereum library.

[![Docs](https://img.shields.io/badge/Documentation-7334f8)](https://docs.sequence.xyz/sdk/unity/overview)
[![Bootstrap](https://img.shields.io/badge/Bootstrap%20your%20Game-7334f8)](https://docs.sequence.xyz/sdk/unity/bootstrap)
[![Build a Game](https://img.shields.io/badge/How%20to%20Build%20a%20Game-7334f8)](https://docs.sequence.xyz/guides/jelly-forest-unity-guide/)

## Demos

Check out our public demos on your device!

[![iOS](https://img.shields.io/badge/iOS-7334f8)](https://testflight.apple.com/join/fgHtPdMb)
[![Android](https://img.shields.io/badge/Android-7334f8)](https://play.google.com/store/apps/details?id=app.sequence.unitysdkdemo)
[![WebGL](https://img.shields.io/badge/WebGL-7334f8)](https://sequence-unity.pages.dev/)

## Supported Platforms

![Unity](https://img.shields.io/badge/Unity%20Engine%202021.3.6f1%20or%20later-6c5d8c)
![Mac](https://img.shields.io/badge/Mac-6c5d8c)
![Windows](https://img.shields.io/badge/Windows-6c5d8c)
![Android](https://img.shields.io/badge/Android-6c5d8c)
![iOS](https://img.shields.io/badge/iOS-6c5d8c)
![WebGL+WebGPU](https://img.shields.io/badge/WebGL-6c5d8c)

## Platform Details

- PC standalone -> (Mono builds only when using OpenIdAuthentication -> the platform specific setup requires system commands that don't work on IL2CPP -> see OpenIdAuthentication.PlatformSpecificSetup)
- Mac standalone -> (Mono builds only when using OpenIdAuthentication -> in our testing MacOS doesn't pick up custom URL schemes automatically unless you run some system commands first; these system commands that don't work on IL2CPP -> see OpenIdAuthentication.PlatformSpecificSetup)

## Contributing

As an open source project, we welcome and encourage your contributions!

Please make sure to increment the package version in `Packages/Sequence-Unity/package.json` according to [Semantic Versioning](https://semver.org/) along with your submissions.

## Testing
The project makes use of Unity's test runner. You can open the test runner window in 
Unity by navigating to `Window > General > Test Runner`.

Many of the tests, specifically those for our custom Ethereum client, make use of a Hardhat-based testchain. This testchain can be found in the root folder of the project - in case you are experiencing issues with it.

Before running tests, boot up the test chain with `make start-testchain`. You may find that you need to stop (control + c) the testchain and restart it when running the test suite again.

### Platform Compile Test
When making large amounts of changes or any changes that may impact builds (assemblies, dependencies, etc.), it is useful to confirm that the SDK still compiles on the [targeted platforms](#supported-platforms). To do this navigate to the top menu and click `Sequence Dev > Platform Compile Test`. This will build the project, and the currently selected scenes in the build settings, on all targeted platforms. All build errors encountered will be recorded in `PlatformCompileTestErrors/<build platform>.txt`. The builds will be cleaned up once completed. This test doesn't run any tests against the individual builds; it only confirms that the project builds on a given platform.

### Testing via command line
It can sometimes be useful to quickly test the project via command line. This can be done without opening Unity or starting the testchain.
#### One-Time Setup
Add this line to your `~/.zshrc` or `~/.bashrc`
`export PATH="/Applications/Unity/Hub/Editor/2021.3.6f1/Unity.app/Contents/MacOS:$PATH"` - note: this is an example path, the exact path may vary based on your system
Then
`source ~/.bashrc` or `source ~/.zshrc`
Then
`touch TestResults.xml` from the route directory of the project

Run `make bootstrap` to install dependancies locally that are required for running the testchain.
#### Running the test
To run the test please use
`make test`
This will automatically start the testchain and open Unity to run the tests. When the tests are finished, the testchain and Unity will be shutdown.
The test results can be found in `TestResults.xml` located in the root directory of the project. The Makefile command will automatically display a summary of the test results.
When a test fails, it is recommended that you open up Unity and test via the usual method.
Note: Please do not run `make test` while you have the project open in Unity - the tests will not run and you will need to `touch TestResults.xml` again.
### Testing the test chain
Occasionally, it may be necessary to test the testchain to 
a) confirm it is giving the behaviours you expect and 
b) to use for comparison with our Unity tests. 
We can safely assume that ethers (which we use to test the testchain) works correctly. To test please use `make test-testchain`. Test output will be in chaintest.out and will also be printed to the terminal. If you need to end the test suite early, use `Control+C` and `make stop`.
*Note: if you already have an instance of Unity running, this will open up a new instance of Unity that will terminate upon completion.

### Troubleshooting
Do you have tests that are failing that you don't think should be or were previously passing and you haven't changed anything?
Here are a few things to try:
1. If you are or were using a debugger, disconnect the debugger from Unity and then reconnect
2. Restart the test chain `Control+C` and `make start-testchain`
3. Restart Unity
* Also note that since tests on the testchain are being run sequentially, if a prior test fails, it may not have unwound properly and may leave the next test in an unexpected state - causing it to fail.

## Samples

The SDK comes with a number of samples that can be imported via `Samples` using the Package Manager. The most important of these is `Setup` which contains a number of Editor scripts and the `SequenceConfig` scriptable object resource that need to live in the `Assets` folder to function correctly with the Unity Editor. 

These live inside the `Samples~` folder as required by the [Package Manager specification](https://docs.unity3d.com/Manual/cus-samples.html). However, the Unity Editor will ignore any folders/files with a '~' character in their name and will not create a `.meta` file for them or import them. In order to facilitate our development, we create a symbolic link named `Samples` that points to the `Samples~` folder - allowing us to see and interact with our Samples and Setup scripts.

Samples include a set of handful feature boilerplate to get started.

How to create a new Boilerplate:
- Create the scripts inside the `Packages/Sequence Embedded Wallet SDK/Sequence/Samples/Setup/Boilerplates/`
- Create a prefab and put it inside the Resources folder in the above directory.
- Create a static function inside `BoilerplateFactory.cs` to instantiate a new instance of the prefab from Resources.
- Add a new `FeatureSelectionButton.cs` inside the `Demo` scene and add a unique `Key` value to the component.

The `Key` value from `FeatureSelectionButton.cs` allows us to only enable selected features in our WebGL demo.
For example, the url `http://localhost:4444/?features=rewards+profile` will only enable the Player Profile and Daily Rewards Boilerplates.
If you don't define any feature in the url, all boilerplates are enabled.

## Assembly Overview

The SDK is split into a number of assemblies with different purposes. Each assembly also has a Test assembly or assembly reference containing tests - this way, our tests aren't included in builds.

### SequenceAuthentication

This contains code related to authentication via Email + OTP, [OIDC](https://openid.net/developers/how-connect-works/), or other means.

### SequenceConfig

Defines the `SequenceConfig` scriptable object and scripts needed to read it. Configuration is done in conjunction with the [Sequence Builder](https://sequence.build/).

### SequenceEcosystemWallet

The integration with our [Ecosystem Wallet](https://docs.sequence.xyz/solutions/wallets/developers/ecosystem-wallet/unity-quickstart). Used to provide users with a seemless and invisible Web3 wallet experience.

### SequenceEmbeddedWallet

The integration with our [Embedded Wallet](https://docs.sequence.xyz/solutions/wallets/developers/embedded-wallet/overview). Used to provide users with a seemless and invisible Web3 wallet experience.

### SequenceEthereum

This is our custom Ethereum library, purpose-built for Unity.

### SequenceIndexer

The integration with our [Indexer API](https://docs.sequence.xyz/api/indexer/overview). Used to quickly index/read on-chain data.

### SequenceMarketplace

The integration with our Marketplace API v2 and Swap API. Used to enable secondary marketplace sales of ERC721/1155s.

### SequencePay

Our implementation of the Pay product line - supporting credit card based fund onboarding and collectible checkout for both primary and secondary sales. Integrations of payment service providers like Transak and Sardine.

### SequenceRelayer

SDK-side extensions to our Sequence Relayer - e.g. transaction queuers.

### SequenceUtils

Universally useful extension methods, helpers, and platform native code used throughout the SDK for a variety of purposes.

## Component Overview - Ethereum Client (SequenceEthereum)
The SDK is broken into a number of components with different responsibilities. This section will give an overview of some of the most important components for users and their intended purposes.

### Client
IEthClient provides an interface for clients. Clients handle the connection to blockchain networks, making various RPC requests. Any time you wish to query the blockchain or submit a transaction, you will need a client. As a rule of thumb, if a method requires a client, you should expect that you will be making a web request and will need to work with async tasks and be prepared to catch any exceptions that are thrown.

### Wallet
EthWallet implements a standard EOA wallet. A wallet keeps track of its own private/public key pair and address and is responsible for providing its private key to the signer when signing transactions.

### Transaction
A transaction, as implemented in EthTransaction, contains all the data and parameters for an EVM transaction. The object is used for initiating its RLP encoding (transactions must be signed and RLP encoded when submitted). Note that all transactions are encoded with a chain Id included to protect against replay attacks, see [EIP-155](https://eips.ethereum.org/EIPS/eip-155).
To make customization even easier, the sample UI comes equipped with a Color Scheme Manager. This monobehaviour script is attached to the `SequenceCanvas` gameObject. By attaching a `ColorScheme` scriptable object and clicking the `Apply` button in the Inspector, the `ColorSchemeManager` will quickly apply the desired color scheme, allowing for faster UI iterations.
To create a `ColorScheme` scriptable object, go to `Assets > Create > Sequence > Color Scheme`. From here, you can give the color scheme a name, move it to the desired directory, and choose your colors.
