#import <Foundation/Foundation.h>
#import <WebKit/WebKit.h>

static WKWebView* webView = nil;

extern "C" {
    void _ShowWebView(const char* url, float x, float y, float width, float height) {
        dispatch_async(dispatch_get_main_queue(), ^{
            if (webView == nil) {
                WKWebViewConfiguration* config = [[WKWebViewConfiguration alloc] init];
                webView = [[WKWebView alloc] initWithFrame:CGRectMake(x, y, width, height) configuration:config];
                
                UIViewController* rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
                [rootVC.view addSubview:webView];
            }
            
            NSString* nsUrl = [NSString stringWithUTF8String:url];
            [webView loadRequest:[NSURLRequest requestWithURL:[NSURL URLWithString:nsUrl]]];
            webView.hidden = NO;
        });
    }

    void _HideWebView() {
        dispatch_async(dispatch_get_main_queue(), ^{
            if (webView != nil) {
                webView.hidden = YES;
            }
        });
    }

    void _RemoveWebView() {
        dispatch_async(dispatch_get_main_queue(), ^{
            if (webView != nil) {
                [webView removeFromSuperview];
                webView = nil;
            }
        });
    }
}