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
		private delegate void BeginResizeCallback();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate Size ResizeCallback(Size size);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void EndResizeCallback();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void CloseRequestedDelegate();

		[DllImport("libmacwindow")] private static extern IntPtr CreateWindow(	IntPtr appPointer, 
																				Size size, 
																				[MarshalAs(UnmanagedType.FunctionPtr)]BeginResizeCallback beginResizeCallback,
																				[MarshalAs(UnmanagedType.FunctionPtr)]ResizeCallback resizeCallback,
																				[MarshalAs(UnmanagedType.FunctionPtr)]BeginResizeCallback endResizeCallback,
																				[MarshalAs(UnmanagedType.FunctionPtr)]CloseRequestedDelegate closeCallback);

		[DllImport("libmacwindow", CharSet = CharSet.Ansi)] private static extern void SetTitle(IntPtr windowPointer, string title);

		[DllImport("libmacwindow")] private static extern void DisposeWindow(IntPtr windowPointer);
		#endregion

		public event Action CloseRequested;
		public event Action BeginResizing;
		public event Action<Size> Resized;
		public event Action EndResizing;

		public readonly IntPtr NativeWindowPointer;
		
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
		public bool IsResizing { get; private set; }
		public Size Size { get; private set; }
		public Size MinSize { get; set; } = new Size(0f, 0f);
		public Size MaxSize { get; set; } = new Size(float.MaxValue, float.MaxValue);

		private bool disposed;
		private string title;

		public NativeWindow(NativeApp app, Size size)
		{
			Size = size;
			NativeWindowPointer = CreateWindow
			(
				app.NativeAppPointer,
				size,
				OnBeginResize,
				OnResize,
				OnEndResize,
				OnCloseRequested
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

		private void OnBeginResize()
		{
			IsResizing = true;
			BeginResizing?.Invoke();
		}

		private Size OnResize(Size size)
		{
			Size = new Size
			(
				size.Width.Clamp(MinSize.Width, MaxSize.Width),
				size.Height.Clamp(MinSize.Height, MaxSize.Height)
			);
			Resized?.Invoke(Size);
			return Size;
		}

		private void OnEndResize()
		{
			IsResizing = false;
			EndResizing?.Invoke();
		}

		private void OnCloseRequested() => CloseRequested?.Invoke();
    }
}