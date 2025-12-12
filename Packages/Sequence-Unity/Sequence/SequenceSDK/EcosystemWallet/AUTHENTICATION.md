# Sequence Ecosystem Wallets - Authentication

We create a local signer and let the user define a set of permissions (or none) to connect that signer and its
permissions to their wallet. Authentication is done through external browsers to open the wallet app for
a specified ecosystem.

## Signers

When creating sessions, we create a local signer and make a request to the wallet app to connect that signer
to our wallet. This signer and its permissions will be included in the sessions topology we get from the 'Tree'
endpoint from the key machine api.

## Redirect Handlers

The application receives response data through a deeplink. 
The SDK automatically generates the url scheme based on the application identifier defined in the unity project.

### Editor

We perform redirects through a localhost port. We define the http://localhost:4444 as the redirect url, which will
send session data.

### iOS

Using the native SafariViewController class to perform in-app requests.

### Android

Using the native Custom Chrome Tabs (CCT) class to perform in-app requests.

### Windows



### MacOS



### WebGL
