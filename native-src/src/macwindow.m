/*
 Simple c interface into a mac-os window, note there is no safety on the
 pointers that are being returned and taken into these utility methods so be very carefull to actually
 provide the correct pointers and to not send pointers that you've allready disposed.
 
 Note: Breaking naming conventions a bit here to match the camal casing of c# methods
 */

#import <Cocoa/Cocoa.h>
#import <Metal/MTLDevice.h>
#import <MetalKit/MTKView.h>

#import "interopTypes.h"
#import "WindowEventListener.h"

extern int * SetupApp()
{
    NSApplication * app = [NSApplication sharedApplication];
    @autoreleasepool
    {
        [app setActivationPolicy:NSApplicationActivationPolicyRegular];
        [app activateIgnoringOtherApps:YES];
        [app finishLaunching];
        
        int * appPointer = (int *) app;
        NSLog(@"[libmacwindow] setup app: %p\n", appPointer);
        return appPointer;
    }
}

extern void ProcessEvents(int * appPointer)
{
    NSApplication * app = (NSApplication *) appPointer;
    @autoreleasepool
    {
        //Find all events that happened since the last call to 'processEvents', then forward the events
        //to the windows associated to this app and then update all the windows
        NSEvent * event;
        while ((event = [NSApp nextEventMatchingMask:~0 untilDate:nil inMode:NSDefaultRunLoopMode dequeue:YES]))
        {
            [app sendEvent: event];
            [app updateWindows];
        }
    }
}

extern void DisposeApp(int * appPointer)
{
    NSApplication * app = (NSApplication *) appPointer;
    @autoreleasepool
    {
        [app release];
        NSLog(@"[libmacwindow] disposed app: %p\n", appPointer);
    }
}

extern int * CreateWindow(int * appPointer, struct Int2 size, char * titleUtf8,
    void * resizedCallback,
    void * beginResizeCallback, void * endResizeCallback,
    void * movedCallback,
    void * minimizedCallback, void * deminizedCallback,
    void * maximizedCallback, void * demaximizedCallback,
    void * closeRequestedCallback)
{
    NSApplication * app = (NSApplication *) appPointer;
    @autoreleasepool
    {
        //Create a centered rect
        NSRect screenRect = [[NSScreen mainScreen] frame];
        NSRect windowRect = NSMakeRect(NSMidX(screenRect) - size.x * .5, NSMidY(screenRect) - size.y * .5, size.x, size.y);
        
        //Setup style for a titled, closable and resizable
        NSUInteger windowStyle = NSWindowStyleMaskTitled | NSWindowStyleMaskClosable | NSWindowStyleMaskResizable | NSWindowStyleMaskMiniaturizable;
        
        //Create the window
        NSWindow * window = [[NSWindow alloc] initWithContentRect:windowRect styleMask:windowStyle backing:NSBackingStoreBuffered defer:NO];
        window.releasedWhenClosed = true; //Auto-release when close
        
        //Set the title
        NSString * titleString = [[NSString alloc] initWithUTF8String:titleUtf8];
        [window setTitle:titleString];
        [titleString release];
        
        //Setup a custom eventlistener for receiving info about what happens to the window
        WindowEventListener * eventListener = [[WindowEventListener alloc] init];
        eventListener->resizedCallback = resizedCallback;
        eventListener->beginResizeCallback = beginResizeCallback;
        eventListener->endResizeCallback = endResizeCallback;
        eventListener->movedCallback = movedCallback;
        eventListener->minimizedCallback = minimizedCallback;
        eventListener->deminizedCallback = deminizedCallback;
        eventListener->maximizedCallback = maximizedCallback;
        eventListener->demaximizedCallback = demaximizedCallback;
        eventListener->closeRequestedCallback = closeRequestedCallback;
        window.delegate = eventListener;
        
        //Attach the window to the app and move it infront
        [window makeKeyAndOrderFront:app];
        
        //Call the resized and moved callback with the initial data
        [eventListener resized:(window)];
        [eventListener moved:(window)];
        
        int * windowPointer = (int *)window;
        NSLog(@"[libmacwindow] created window: %p\n", windowPointer);
        return windowPointer;
    }
}

extern int * CreateMetalView(int * windowPointer)
{
    NSWindow * window = (NSWindow *)windowPointer;
    @autoreleasepool
    {
        //Create a device handle
        id<MTLDevice> metalDevice = MTLCreateSystemDefaultDevice();
        
        //Create view that is attached to the window
        CGRect frameBounds = CGRectMake(0, 0, 1, 1);
        MTKView * metalView = [[MTKView alloc] initWithFrame:frameBounds device:metalDevice];
        metalView.paused = YES;
        metalView.enableSetNeedsDisplay = NO;
        [window setContentView:metalView];
        
        int * viewPointer = (int *)metalView;
        NSLog(@"[libmacwindow] created metal-view: %p\n", viewPointer);
        return viewPointer;
    }
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
        NSLog(@"[libmacwindow] disposed window: %p\n", windowPointer);
    }
}
