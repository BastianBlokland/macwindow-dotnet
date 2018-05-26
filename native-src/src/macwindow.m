/*
 Simple c interface into a mac-os window, note there is no safety on the
 pointer that are being returned and taken into these utility methods so be very carefull to actually
 provide the correct pointers and to not send pointers that you've allready disposed.
 
 Note: Breaking naming conventions a bit here to match the camal casing of c# methods
 */

#import <Cocoa/Cocoa.h>
#import "interopTypes.h"
#import "WindowEventListener.h"

extern int * SetupApp()
{
    NSApplication * app = [NSApplication sharedApplication];
    [app setActivationPolicy:NSApplicationActivationPolicyRegular];
    [app activateIgnoringOtherApps:YES];
    [app finishLaunching];
    
    int * appPointer = (int *) app;
    NSLog(@"[libmacwindow] setup app: %p\n", appPointer);
    return appPointer;
}

extern void ProcessEvents(int * appPointer)
{
    NSApplication * app = (NSApplication *) appPointer;
    
    //Find all events that happened since the last call to 'processEvents', then forward the events
    //to the windows associated to this app and then update all the windows
    NSEvent * event;
    while ((event = [NSApp nextEventMatchingMask:~0 untilDate:nil inMode:NSDefaultRunLoopMode dequeue:YES]))
    {
        [app sendEvent: event];
        [app updateWindows];
    }
}

extern void DisposeApp(int * appPointer)
{
    NSApplication * app = (NSApplication *) appPointer;
    @autoreleasepool
    {
        [app release];
    }
    NSLog(@"[libmacwindow] disposed app: %p\n", appPointer);
}

extern int * CreateWindow(int * appPointer, struct Size size,
    void * beginResizeCallback,
    void * resizeCallback,
    void * endResizeCallback,
    void * closeRequestedCallback)
{
    NSApplication * app = (NSApplication *) appPointer;
    
    //Create a centered rect
    NSRect screenRect = [[NSScreen mainScreen] frame];
    NSRect windowRect = NSMakeRect(NSMidX(screenRect) - size.width * .5, NSMidY(screenRect) - size.height * .5, size.width, size.height);
    
    //Setup style for a titled, closable and resizable
    NSUInteger windowStyle = NSWindowStyleMaskTitled | NSWindowStyleMaskClosable | NSWindowStyleMaskResizable;
    
    //Create the window
    NSWindow * window = [[NSWindow alloc] initWithContentRect:windowRect styleMask:windowStyle backing:NSBackingStoreBuffered defer:NO];
    
    //Setup a custom eventlistener for receiving info about what happens to the window
    WindowEventListener * eventListener = [[WindowEventListener alloc] init];
    eventListener->beginResizeCallback = beginResizeCallback;
    eventListener->resizeCallback = resizeCallback;
    eventListener->endResizeCallback = endResizeCallback;
    eventListener->closeRequestedCallback = closeRequestedCallback;
    window.delegate = eventListener;
    
    //Attach the window to the app and move it infront
    [window makeKeyAndOrderFront:app];
    
    int * windowPointer = (int *)window;
    NSLog(@"[libmacwindow] created window: %p\n", windowPointer);
    return windowPointer;
}

extern void SetTitle(int * windowPointer, char * titleUtf8)
{
    NSWindow * window = (NSWindow *)windowPointer;
    @autoreleasepool
    {
        NSString * titleString = [[NSString alloc] initWithUTF8String:titleUtf8];
        [window setTitle:titleString];
        [titleString release];
    }
}

extern void DisposeWindow(int * windowPointer)
{
    NSWindow * window = (NSWindow *)windowPointer;
    @autoreleasepool
    {
        [window close];
        [window.delegate release]; //Releasse the 'WindowEventListener' we allocated when we created the window
        [window release];
    }
    NSLog(@"[libmacwindow] disposed window: %p\n", windowPointer);
}
