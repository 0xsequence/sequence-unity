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

### Testing via command line
It can sometimes be useful to quickly test the project via command line. This can be done without opening Unity or starting the testchain.
#### One-Time Setup
Add this line to your `~/.zshrc` or `~/.bashrc`
`export PATH="/Applications/Unity/Hub/Editor/2021.3.6f1/Unity.app/Contents/MacOS:$PATH"` - note: this is an example path, the exact path may vary based on your system
Then
`source ~/.bashrc` or `source ~/.zshrc`
Then
`touch TestResults.xml` from the route directory of the project
#### Running the test
To run the test please use
`make test`
This will automatically start the testchain and open Unity to run the tests. When the tests are finished, the testchain and Unity will be shutdown.
The test results can be found in `TestResults.xml` located in the root directory of the project. The Makefile command will automatically display a summary of the test results.
When a test fails, it is recommended that you open up Unity and test via the usual method.
### Testing the test chain
Occasionally, it may be necessary to test the testchain to a) confirm it is 
giving the behaviours you expect and b) to use for comparison with our 
Unity tests. We can safely assume that ethers (which we use to test the 
testchain) works correctly. To test please use `make test-testchain`. Test output will be in 
chaintest.out and will also be printed to the terminal. If you need to end the test suite 
early, use `Control+C` and `make stop`.
*Note: if you already have an instance of Unity running, this will open up a new instance of Unity that will terminate upon completion.

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
