#import <Foundation/Foundation.h>
#import "WindowEventListener.h"

@implementation WindowEventListener

- (void) windowDidResize: (NSNotification *)notification
{
    NSWindow * window = (NSWindow *)notification.object;
    [self resized:(window)];
}

- (void) windowWillStartLiveResize: (NSNotification *)notification
{
    if(beginResizeCallback != nil)
        beginResizeCallback();
}

- (void) windowDidEndLiveResize: (NSNotification *)notification
{
    if(endResizeCallback != nil)
        endResizeCallback();
}

- (void) windowDidMove: (NSNotification *)notification
{
    NSWindow * window = (NSWindow *)notification.object;
    [self moved:(window)];
}

- (void) windowDidMiniaturize: (NSNotification *)notification
{
    if(minimizedCallback != nil)
        minimizedCallback();
    
    if(movedCallback != nil)
    {
        //Bit of windows convention here to set the position of a minimised window to -32000
        struct Int2 pos;
        pos.x = -32000;
        pos.y = -32000;
        movedCallback(pos);
    }
    if(resizedCallback != nil)
    {
        struct Int2 size;
        size.x = 0;
        size.y = 0;
        resizedCallback(size);
    }
}

- (void) windowDidDeminiaturize: (NSNotification *)notification
{
    if(deminizedCallback != nil)
        deminizedCallback();
    
    //Also notify about a move and resize
    NSWindow * window = (NSWindow *)notification.object;
    [self moved:(window)];
    [self resized:(window)];
}

- (void) windowDidEnterFullScreen: (NSNotification *)notification
{
    if(maximizedCallback != nil)
        maximizedCallback();
    
    //Also notify about a move and resize
    NSWindow * window = (NSWindow *)notification.object;
    [self moved:(window)];
    [self resized:(window)];
}

- (void) windowDidExitFullScreen: (NSNotification *)notification
{
    if(demaximizedCallback != nil)
        demaximizedCallback();
    
    //Also notify about a move and resize
    NSWindow * window = (NSWindow *)notification.object;
    [self moved:(window)];
    [self resized:(window)];
}

- (BOOL) windowShouldClose: (NSWindow *)sender
{
    if(closeRequestedCallback != nil)
        closeRequestedCallback();
    
    //Return false to stop the closing, this allows us to forward the message to the application
    //so it can perform cleanup before it decides to dispose the window
    return 0;
}

- (void) resized: (NSWindow *)window
{
    if(resizedCallback != nil)
    {
        NSRect contentRect = window.contentView.frame;
        
        struct Int2 size;
        size.x = contentRect.size.width;
        size.y = contentRect.size.height;
        resizedCallback(size);
    }
}

- (void) moved: (NSWindow *)window
{
    if(movedCallback != nil)
    {
        NSRect contentRect = window.contentView.frame;
        
        struct Int2 pos;
        pos.x = window.frame.origin.x;
        //Note inverting the y to be top = 0 and screen size is bottom.
        //This makes it match better consistent with how win32 handles rectangles
        pos.y = window.screen.frame.size.height - contentRect.size.height - window.frame.origin.y;
        movedCallback(pos);
    }
}

@end
