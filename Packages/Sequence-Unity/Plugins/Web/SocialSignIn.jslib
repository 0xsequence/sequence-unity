mergeInto(LibraryManager.library, {
    
    GoogleSignIn: function(googleClientId, nonce) {
        window.dispatchReactUnityEvent("GoogleSignIn", UTF8ToString(googleClientId), UTF8ToString(nonce));
    },
    
    OpenWalletApp: function(url) {
        window.dispatchReactUnityEvent("OpenWalletApp", UTF8ToString(url));
    },
    
    GetPageOrigin: function () {
      var origin = window.location.origin;
      var buffer = allocate(intArrayFromString(origin), 'i8', ALLOC_STACK);
      return buffer;
    }
    
});
