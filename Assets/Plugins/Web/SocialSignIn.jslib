mergeInto(LibraryManager.library, {

    SocialSignIn: function (url) {
        var authUrl = UTF8ToString(url);
        var left = (window.innerWidth - 600) / 2;
        var top = (window.innerHeight - 600) / 2;
        var popup = window.open(authUrl, 'authPopup', 'width=600,height=600,left=' + left + ',top=' + top);
    },
    
});
