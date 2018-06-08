#import <Cocoa/Cocoa.h>
#import "interopTypes.h"

@interface WindowEventListener : NSObject <NSWindowDelegate>
{
    @public void (* resizedCallback)(struct Int2);
                  
    @public void (* beginResizeCallback)(void);
    @public void (* endResizeCallback)(void);
    
    @public void (* movedCallback)(struct Int2);
                  
    @public void (* minimizedCallback)(void);
    @public void (* deminizedCallback)(void);
                  
    @public void (* maximizedCallback)(void);
    @public void (* demaximizedCallback)(void);
    
    @public void (* closeRequestedCallback)(void);
}

- (void) windowDidResize: (NSNotification *)notification;
- (void) windowWillStartLiveResize: (NSNotification *)notification;
- (void) windowDidEndLiveResize: (NSNotification *)notification;
- (void) windowDidMove: (NSNotification *)notification;
- (void) windowDidMiniaturize: (NSNotification *)notification;
- (void) windowDidDeminiaturize: (NSNotification *)notification;
- (void) windowDidEnterFullScreen: (NSNotification *)notification;
- (void) windowDidExitFullScreen: (NSNotification *)notification;
- (BOOL) windowShouldClose: (NSWindow *)sender;

- (void) resized:(NSWindow *)window;
- (void) moved:(NSWindow *)window;

@end
