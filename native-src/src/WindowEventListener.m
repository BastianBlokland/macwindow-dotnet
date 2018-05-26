#import <Foundation/Foundation.h>
#import "WindowEventListener.h"

@implementation WindowEventListener

- (void) windowWillStartLiveResize: (NSNotification *)notification
{
    if(beginResizeCallback != nil)
        beginResizeCallback();
}

- (NSSize) windowWillResize: (NSWindow *)sender toSize: (NSSize)frameSize
{
    if(resizeCallback != nil)
    {
        struct Size size;
        size.width = frameSize.width;
        size.height = frameSize.height;
        size = resizeCallback(size);
        return NSMakeSize(size.width, size.height);
    }
    return frameSize;
}

- (void) windowDidEndLiveResize: (NSNotification *)notification
{
    if(endResizeCallback != nil)
        endResizeCallback();
}

- (BOOL) windowShouldClose: (NSWindow *)sender
{
    if(closeRequestedCallback != nil)
        closeRequestedCallback();
    
    //Return false to stop the closing, this allows us to forward the message to the application
    //so it can perform cleanup before it decides to dispose the window
    return 0;
}
@end
