using System;
using System.Runtime.InteropServices;

namespace NativeMacOS
{
    /// <summary>
    /// Wrapper around the NSWindow object, can be used to set properties of the window and get callbacks when the user interacts with
    /// the window.
    /// Note: Multiple windows can be created attached to the same app.
    /// </summary>
    public class NativeWindow : IDisposable
    {
        #region Native bindings
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ResizedDelegate(Int2 size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void BeginResizeDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void EndResizeDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void MovedDelegate(Int2 size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void MinimizedDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DeminimizedDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void MaximizedDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DemaximizedDelegate();
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void CloseRequestedDelegate();

        [DllImport("libmacwindow", CharSet = CharSet.Ansi)] 
        private static extern IntPtr CreateWindow(	IntPtr appPointer,
                                                    Int2 size, Int2 minSize,
                                                    string title,
                                                    [MarshalAs(UnmanagedType.FunctionPtr)]ResizedDelegate resizedCallback,
                                                    [MarshalAs(UnmanagedType.FunctionPtr)]BeginResizeDelegate beginResizeCallback,
                                                    [MarshalAs(UnmanagedType.FunctionPtr)]EndResizeDelegate endResizeCallback,
                                                    [MarshalAs(UnmanagedType.FunctionPtr)]MovedDelegate movedCallback,
                                                    [MarshalAs(UnmanagedType.FunctionPtr)]MinimizedDelegate minimizedCallback,
                                                    [MarshalAs(UnmanagedType.FunctionPtr)]DeminimizedDelegate deminimizedDelegate,
                                                    [MarshalAs(UnmanagedType.FunctionPtr)]MaximizedDelegate maximizedCallback,
                                                    [MarshalAs(UnmanagedType.FunctionPtr)]DemaximizedDelegate demaximizedCallback,
                                                    [MarshalAs(UnmanagedType.FunctionPtr)]CloseRequestedDelegate closeCallback);

        [DllImport("libmacwindow", CharSet = CharSet.Ansi)] 
        private static extern void SetTitle(IntPtr windowPointer, string title);

        [DllImport("libmacwindow")] 
        private static extern void DisposeWindow(IntPtr windowPointer);
        #endregion

        public event Action CloseRequested;
        public event Action<Int2> Resized;
        public event Action<Int2> Moved;
        
        public string Title
        {
            get => title;
            set
            {
                if(title != value)
                {
                    SetTitle(NativeWindowPointer, value);
                    title = value;
                }
            }
        }
        public bool Minimized { get; private set; }
        public bool Maximized { get; private set; }
        public bool IsResizing { get; private set; }
        public IntRect Rect { get; private set; }
        public readonly IntPtr NativeWindowPointer;

        //Handle to the delgates we've handed to the native side, its important we hold a handle
        //to them otherwise they will get garbage collected on the native side
        private readonly ResizedDelegate onResized;
        private readonly BeginResizeDelegate onBeginResize;
        private readonly EndResizeDelegate onEndResize;
        private readonly MovedDelegate onMoved;
        private readonly MinimizedDelegate onMinimized;
        private readonly DeminimizedDelegate onDeminimized;
        private readonly MaximizedDelegate onMaximized;
        private readonly DemaximizedDelegate onDemaximized;
        private readonly CloseRequestedDelegate onCloseRequested;

        private bool disposed;
        private string title;

        public NativeWindow(NativeApp app, Int2 size, Int2 minSize, string title)
        {
            this.title = title;

            //Create delegates pointing to our callbacks
            //NOTE: Its important to keep a reference to this delegates in the class because
            //otherwise they will be garbage collected but the native side still has pointer to them
            onResized = new ResizedDelegate(OnResized);
            onBeginResize = new BeginResizeDelegate(OnBeginResize);
            onEndResize = new EndResizeDelegate(OnEndResize);
            onMoved = new MovedDelegate(OnMoved);
            onMinimized = new MinimizedDelegate(OnMinimized);
            onDeminimized = new DeminimizedDelegate(OnDeminimized);
            onMaximized = new MaximizedDelegate(OnMaximized);
            onDemaximized = new DemaximizedDelegate(OnDemaximized);
            onCloseRequested = new CloseRequestedDelegate(OnCloseRequested);

            NativeWindowPointer = CreateWindow
            (
                app.NativeAppPointer,
                size,
                minSize,
                title,
                onResized,
                onBeginResize,
                onEndResize,
                onMoved,
                onMinimized, 
                onDeminimized,
                onMaximized,
                onDemaximized,
                onCloseRequested
            );
        }

        public void Dispose()
        {
            if(!disposed)
            {
                DisposeWindow(NativeWindowPointer);
                disposed = true;
            }
        }

        private void OnResized(Int2 size)
        {
            Rect = new IntRect(Rect.Min, Rect.Min + size);
            Resized?.Invoke(size);
        }

        private void OnBeginResize() => IsResizing = true;

        private void OnEndResize() => IsResizing = false;

        private void OnMoved(Int2 pos)
        {
            Rect = new IntRect(pos, pos + Rect.Size);
            Moved?.Invoke(pos);
        }

        private void OnMinimized() => Minimized = true;
        
        private void OnDeminimized() => Minimized = false;

        private void OnMaximized() => Maximized = true;

        private void OnDemaximized() => Maximized = false;

        private void OnCloseRequested() => CloseRequested?.Invoke();
    }
}