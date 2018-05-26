#import <Cocoa/Cocoa.h>

@interface WindowEventListener : NSObject <NSWindowDelegate>

- (id) initWithCallbacks: (void *) closeRequestedCallback;

- (BOOL) windowShouldClose: (NSWindow *) sender;

@end
