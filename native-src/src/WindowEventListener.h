#import <Cocoa/Cocoa.h>
#import "interopTypes.h"

@interface WindowEventListener : NSObject <NSWindowDelegate>
{
    @public struct Size (* resizeCallback)(struct Size);
    @public void (* closeRequestedCallback)(void);
}

- (NSSize) windowWillResize: (NSWindow *)sender toSize: (NSSize)frameSize;

- (BOOL) windowShouldClose: (NSWindow *)sender;

@end
