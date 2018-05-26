#import <Cocoa/Cocoa.h>
#import "interopTypes.h"

@interface WindowEventListener : NSObject <NSWindowDelegate>
{
    @public void (* beginResizeCallback)(void);
    @public struct Size (* resizeCallback)(struct Size);
    @public void (* endResizeCallback)(void);
    
    @public void (* closeRequestedCallback)(void);
}

- (void) windowWillStartLiveResize: (NSNotification *)notification;
- (NSSize) windowWillResize: (NSWindow *)sender toSize: (NSSize)frameSize;
- (void) windowDidEndLiveResize: (NSNotification *)notification;

- (BOOL) windowShouldClose: (NSWindow *)sender;

@end
