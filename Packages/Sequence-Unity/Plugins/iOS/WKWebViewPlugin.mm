// WKWebViewPlugin.mm
// This file should be placed in your Unity project under Assets/Plugins/iOS/

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>
#import <objc/runtime.h>

@interface WebViewLoggerDelegate : NSObject <WKNavigationDelegate>
@end

@implementation WebViewLoggerDelegate

- (void)webView:(WKWebView *)webView didStartProvisionalNavigation:(WKNavigation *)navigation {
    NSLog(@"WKWebViewPlugin: Starting to load: %@", webView.URL.absoluteString);
}

- (void)webView:(WKWebView *)webView didFinishNavigation:(WKNavigation *)navigation {
    NSLog(@"WKWebViewPlugin: Finished loading: %@", webView.URL.absoluteString);
}

- (void)webView:(WKWebView *)webView didFailNavigation:(WKNavigation *)navigation withError:(NSError *)error {
    NSLog(@"WKWebViewPlugin: Failed to load: %@, error: %@", webView.URL.absoluteString, error.localizedDescription);
}

@end

// Extern C block to make these functions callable from C#
extern "C" {

    // Function to open a URL in a WKWebView
    void _OpenURLInWKWebView(const char* urlString) {
        // Get the main thread to perform UI operations
        dispatch_async(dispatch_get_main_queue(), ^{
            // Convert C string to NSString
            NSString *nsUrlString = [NSString stringWithUTF8String:urlString];
            NSURL *url = [NSURL URLWithString:nsUrlString];
            
            NSLog(@"WKWebViewPlugin: Received URL string (C char*): %s", urlString);
            NSLog(@"WKWebViewPlugin: Converted URL string (NSString): %@", nsUrlString);

            // Get the current Unity view controller
            // UnityGetViewController() is a Unity-provided function to get the root view controller
            UIViewController *unityViewController = UnityGetGLViewController();

            if (unityViewController) {
                // Configure WKWebView
                WKWebViewConfiguration *config = [[WKWebViewConfiguration alloc] init];
                WKWebView *webView = [[WKWebView alloc] initWithFrame:CGRectZero configuration:config];
                webView.autoresizingMask = UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight;
                
                // Create and attach the navigation delegate
                WebViewLoggerDelegate *loggerDelegate = [[WebViewLoggerDelegate alloc] init];
                webView.navigationDelegate = loggerDelegate;

                // Create a UIViewController to host the WKWebView
                UIViewController *webViewController = [[UIViewController alloc] init];
                webViewController.view.backgroundColor = [UIColor whiteColor]; // Set background color

                // Add the WKWebView to the webViewController's view
                [webViewController.view addSubview:webView];
                
                // Retain the delegate by associating it with the controller
                objc_setAssociatedObject(webViewController, @"loggerDelegate", loggerDelegate, OBJC_ASSOCIATION_RETAIN_NONATOMIC);

                // Set constraints for the webView to fill the webViewController's view
                webView.translatesAutoresizingMaskIntoConstraints = NO;
                [NSLayoutConstraint activateConstraints:@[
                    [webView.topAnchor constraintEqualToAnchor:webViewController.view.safeAreaLayoutGuide.topAnchor],
                    [webView.bottomAnchor constraintEqualToAnchor:webViewController.view.safeAreaLayoutGuide.bottomAnchor],
                    [webView.leadingAnchor constraintEqualToAnchor:webViewController.view.safeAreaLayoutGuide.leadingAnchor],
                    [webView.trailingAnchor constraintEqualToAnchor:webViewController.view.safeAreaLayoutGuide.trailingAnchor]
                ]];

                // Add a "Done" button to dismiss the web view
                UIButton *doneButton = [UIButton buttonWithType:UIButtonTypeSystem];
                [doneButton setTitle:@"Done" forState:UIControlStateNormal];
                [doneButton addTarget:webViewController action:@selector(dismissWebView) forControlEvents:UIControlEventTouchUpInside];
                doneButton.translatesAutoresizingMaskIntoConstraints = NO;
                [webViewController.view addSubview:doneButton];

                // Position the done button (e.g., top right)
                [NSLayoutConstraint activateConstraints:@[
                    [doneButton.topAnchor constraintEqualToAnchor:webViewController.view.safeAreaLayoutGuide.topAnchor constant:10],
                    [doneButton.trailingAnchor constraintEqualToAnchor:webViewController.view.safeAreaLayoutGuide.trailingAnchor constant:-10]
                ]];
                
                // Set the done button's content edge insets for better touch target
                doneButton.contentEdgeInsets = UIEdgeInsetsMake(5, 10, 5, 10);
                doneButton.layer.cornerRadius = 8;
                doneButton.backgroundColor = [UIColor colorWithWhite:0.9 alpha:0.8]; // Slightly transparent background

                // Load the URL
                if (url) {
                    [webView loadRequest:[NSURLRequest requestWithURL:url]];
                } else {
                    NSLog(@"WKWebViewPlugin: Invalid URL string provided: %@", nsUrlString);
                }

                // Present the web view controller modally
                [unityViewController presentViewController:webViewController animated:YES completion:nil];

            } else {
                NSLog(@"WKWebViewPlugin: Could not get Unity view controller.");
            }
        });
    }

} // extern "C"

// Forward declaration for UnityGetViewController (provided by Unity's iOS framework)
// This function is defined in Unity's AppController.h, which is automatically linked.
UIViewController* UnityGetViewController();
