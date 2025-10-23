// ASWebAuth.mm
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <AuthenticationServices/AuthenticationServices.h>
#import "UnityInterface.h"

static ASWebAuthenticationSession *g_session = nil;
static char g_gameObject[256] = {0};
static char g_callbackMethod[256] = {0};

@interface ASWebAuthAnchorProvider : NSObject <ASWebAuthenticationPresentationContextProviding>
@end

@implementation ASWebAuthAnchorProvider
- (ASPresentationAnchor)presentationAnchorForWebAuthenticationSession:(ASWebAuthenticationSession *)session
{
    UIWindow *keyWindow = nil;

    if (@available(iOS 13.0, *)) {
        for (UIWindowScene *scene in [UIApplication sharedApplication].connectedScenes) {
            if (scene.activationState == UISceneActivationStateForegroundActive) {
                for (UIWindow *window in scene.windows) {
                    if (window.isKeyWindow) { keyWindow = window; break; }
                }
                if (keyWindow) break;
            }
        }
    } else {
        keyWindow = [UIApplication sharedApplication].keyWindow;
    }

    return keyWindow ?: [[UIWindow alloc] initWithFrame:[UIScreen mainScreen].bounds];
}
@end

static ASWebAuthAnchorProvider *g_anchorProvider;

extern "C" {

static void SendUnityMessage(const char *payload)
{
    UnitySendMessage("SequenceNativeReceiver", "HandleResponse", payload ? payload : "");
}

void ASWebAuth_Start(const char *urlCString, const char *callbackSchemeCString)
{
    if (@available(iOS 12.0, *)) {
    
        NSString *urlStr = urlCString ? [NSString stringWithUTF8String:urlCString] : @"";
        NSString *schemeStr = callbackSchemeCString ? [NSString stringWithUTF8String:callbackSchemeCString] : nil;
    
        dispatch_async(dispatch_get_main_queue(), ^{
            if (g_session) {
                [g_session cancel];
                g_session = nil;
            }

            NSURL *url = [NSURL URLWithString:urlStr];
            
            if (url == nil || urlStr.length == 0) {
                NSString *msg = [NSString stringWithFormat:@"ERROR: invalid URL = %@", urlStr ?: @"(null)"];
                SendUnityMessage(msg.UTF8String);
                return;
            }

            g_anchorProvider = [ASWebAuthAnchorProvider new];

            g_session = [[ASWebAuthenticationSession alloc]
                         initWithURL:url
                         callbackURLScheme:schemeStr
                         completionHandler:^(NSURL * _Nullable callbackURL, NSError * _Nullable error) {

                if (error) {
                    // ASWebAuthenticationSessionErrorCodeCanceledLogin == 1
                    NSString *msg = [NSString stringWithFormat:@"ERROR:%ld:%@", (long)error.code, error.localizedDescription ?: @"Unknown"];
                    SendUnityMessage(msg.UTF8String);
                } else if (callbackURL) {
                    SendUnityMessage(callbackURL.absoluteString.UTF8String);
                } else {
                    SendUnityMessage("ERROR:unknown");
                }

                g_session = nil;
            }];

            if (@available(iOS 13.0, *)) {
                g_session.presentationContextProvider = g_anchorProvider;
                // Optional: use ephemeral browser session (no shared cookies)
                // g_session.prefersEphemeralWebBrowserSession = YES;
            }

            BOOL started = [g_session start];
            if (!started) {
                SendUnityMessage("ERROR:failed_to_start");
                g_session = nil;
            }
        });
    } else {
        SendUnityMessage("ERROR:iOS_version_unsupported");
    }
}

void ASWebAuth_Cancel()
{
    if (@available(iOS 12.0, *)) {
        dispatch_async(dispatch_get_main_queue(), ^{
            if (g_session) {
                [g_session cancel];
                g_session = nil;
                SendUnityMessage("CANCELED");
            }
        });
    }
}

} // extern "C"
