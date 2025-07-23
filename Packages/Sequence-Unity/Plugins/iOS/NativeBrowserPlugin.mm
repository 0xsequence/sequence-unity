#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <SafariServices/SafariServices.h>
#import <objc/runtime.h>
#import <objc/message.h>

static SFSafariViewController *safariVC = nil;

extern "C" {
    void OpenWalletApp(const char *urlCString)
    {
        NSString *urlString = [NSString stringWithUTF8String:urlCString];
        NSURL *url = [NSURL URLWithString:urlString];
        if (!url) return;

        UIViewController *rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
        safariVC = [[SFSafariViewController alloc] initWithURL:url];

        NSLog(@"[SafariPlugin] Presenting SFSafariViewController with URL: %@", urlString);

        dispatch_async(dispatch_get_main_queue(), ^{
            [rootVC presentViewController:safariVC animated:YES completion:nil];
        });
    }
}

@interface DeepLinkListener : NSObject
@end

@implementation DeepLinkListener

+ (void)load
{
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(setupDeepLinkHandler)
                                                 name:UIApplicationDidFinishLaunchingNotification
                                               object:nil];
}

+ (void)setupDeepLinkHandler
{
    Class appDelegateClass = [UIApplication sharedApplication].delegate.class;

    SEL originalSelector  = @selector(application:openURL:options:);
    SEL swizzledSelector  = @selector(my_application:openURL:options:);

    Method originalMethod = class_getInstanceMethod(appDelegateClass, originalSelector);
    Method swizzledMethod = class_getInstanceMethod(self, swizzledSelector);

    BOOL didAdd = class_addMethod(appDelegateClass,
                                  originalSelector,
                                  method_getImplementation(swizzledMethod),
                                  method_getTypeEncoding(swizzledMethod));

    if (!didAdd)
    {
        method_exchangeImplementations(originalMethod, swizzledMethod);
    }
}

- (BOOL)my_application:(UIApplication *)app
               openURL:(NSURL *)url
               options:(NSDictionary<UIApplicationOpenURLOptionsKey, id> *)options
{
    NSLog(@"[SafariPlugin] Deep link received: %@", url.absoluteString);

    UnitySendMessage("SequenceNativeReceiver", "HandleResponse", url.absoluteString.UTF8String);

    if (safariVC)
    {
        NSLog(@"[SafariPlugin] Dismissing SFSafariViewController");
        dispatch_async(dispatch_get_main_queue(), ^{
            [safariVC dismissViewControllerAnimated:YES completion:nil];
            safariVC = nil;
        });
    }
    return YES;
}

@end
