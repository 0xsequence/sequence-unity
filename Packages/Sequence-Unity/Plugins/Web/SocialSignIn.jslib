mergeInto(LibraryManager.library, {
    
    GoogleSignIn: function(googleClientId, nonce) {
        window.dispatchReactUnityEvent("GoogleSignIn", UTF8ToString(googleClientId), UTF8ToString(nonce));
    },
    
    OpenWalletApp: function(url) {
        window.dispatchReactUnityEvent("OpenWalletApp", UTF8ToString(url));
    },
    
});
