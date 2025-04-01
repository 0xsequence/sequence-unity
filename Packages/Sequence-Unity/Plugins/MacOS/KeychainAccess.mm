//
//  KeychainAccess.mm
//
//  Created by Quinn Purdy on 2024-06-18. Updated on 2025-04-01 by Quinn Purdy
//
// To update the libKeychainAccess.dylib, run the following command from the root of this directory:
// clang -arch x86_64 -arch arm64 -dynamiclib -framework Foundation -framework Security \
         -o libKeychainAccess.dylib KeychainAccess.mm

#import <Foundation/Foundation.h>
#import <Security/Security.h>

@interface KeychainAccess : NSObject

+ (void)saveKeychainValue:(NSString *)value forKey:(NSString *)key;
+ (NSString *)getKeychainValueForKey:(NSString *)key;

@end

@implementation KeychainAccess

+ (void)saveKeychainValue:(NSString *)value forKey:(NSString *)key {
    NSData *data = [value dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *query = @{(__bridge id)kSecClass: (__bridge id)kSecClassGenericPassword,
                            (__bridge id)kSecAttrAccount: key};

    // Check if the item already exists
    CFTypeRef result = NULL;
    OSStatus status = SecItemCopyMatching((__bridge CFDictionaryRef)query, &result);

    if (status == errSecSuccess) {
        // Item exists, delete it
        SecItemDelete((__bridge CFDictionaryRef)query);
    }

    // Add the new item
    query = @{(__bridge id)kSecClass: (__bridge id)kSecClassGenericPassword,
              (__bridge id)kSecAttrAccount: key,
              (__bridge id)kSecValueData: data};
    SecItemAdd((__bridge CFDictionaryRef)query, NULL);
}

+ (NSString *)getKeychainValueForKey:(NSString *)key {
    NSDictionary *query = @{(__bridge id)kSecClass: (__bridge id)kSecClassGenericPassword,
                            (__bridge id)kSecAttrAccount: key,
                            (__bridge id)kSecReturnData: (__bridge id)kCFBooleanTrue,
                            (__bridge id)kSecMatchLimit: (__bridge id)kSecMatchLimitOne};
    CFTypeRef result = NULL;
    SecItemCopyMatching((__bridge CFDictionaryRef)query, &result);
    NSData *data = (__bridge_transfer NSData *)result;
    if (data) {
        return [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
    }
    return nil;
}

@end

extern "C" {
    void SaveKeychainValue(const char* key, const char* value) {
        @autoreleasepool {
            NSString *keyString = [NSString stringWithUTF8String:key];
            NSString *valueString = [NSString stringWithUTF8String:value];
            [KeychainAccess saveKeychainValue:valueString forKey:keyString];
        }
    }

    const char* GetKeychainValue(const char* key) {
        @autoreleasepool {
            NSString *keyString = [NSString stringWithUTF8String:key];
            NSString *valueString = [KeychainAccess getKeychainValueForKey:keyString];
            if (valueString) {
                const char* cString = [valueString UTF8String];
                return strdup(cString); // Duplicate the C string memory
            } else {
                return NULL;
            }
        }
    }
}
