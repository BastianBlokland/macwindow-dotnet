using System;
using System.Runtime.InteropServices;

namespace NativeMacOS
{
    /// <summary>
    /// Native bindings around a NSApplication, this is root of the application and handles processing all the input events
    /// and updating all the windows that are attached to this app.
    /// NOTE: The lifetime of the app has the be longer then the lifetime of the windows it contains. Only one app can exist at a time
    /// </summary>
    public class NativeApp : IDisposable
    {
        #region Native bindings
        [DllImport("libmacwindow")] 
        private static extern IntPtr SetupApp();

        [DllImport("libmacwindow")] 
        private static extern void ProcessEvents(IntPtr appPointer);

        [DllImport("libmacwindow")] 
        private static extern void DisposeApp(IntPtr appPointer);
        #endregion

        public readonly IntPtr NativeAppPointer;

        private bool disposed;

        public NativeApp() => NativeAppPointer = SetupApp();

        public void ProcessEvents()
        {
            if(disposed)
                throw new Exception($"[{nameof(NativeApp)}] Allready disposed!");
            ProcessEvents(NativeAppPointer);
        }

        public void Dispose()
        {
            if(!disposed)
            {
                DisposeApp(NativeAppPointer);
                disposed = true;
            }
        }
    }
}
