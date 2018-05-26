#import <Foundation/Foundation.h>
#import "WindowEventListener.h"

@implementation WindowEventListener
{
    void (* closeRequestedCallback)(void);
}

- (id) initWithCallbacks: (void *) closeRequestedCallback
{
    self = [super init];
    if (self)
    {
        self->closeRequestedCallback = closeRequestedCallback;
    }
    return self;
}

- (BOOL) windowShouldClose: (NSWindow *) sender
{
    if(closeRequestedCallback != nil)
        closeRequestedCallback();
    
    //Return false to stop the closing, this allows us to forward the message to the application
    //so it can perform cleanup before it decides to dispose the window
    return 0;
}
@end
