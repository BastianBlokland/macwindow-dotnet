using System;
using System.Runtime.InteropServices;

namespace MacOS
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
		private delegate void CloseRequestedDelegate();

		[DllImport("libmacwindow")] private static extern IntPtr CreateWindow(	IntPtr appPointer, 
																				float width, 
																				float height, 
																				[MarshalAs(UnmanagedType.FunctionPtr)]CloseRequestedDelegate closeCallback);

		[DllImport("libmacwindow", CharSet = CharSet.Ansi)] private static extern void SetTitle(IntPtr windowPointer, string title);

		[DllImport("libmacwindow")] private static extern void DisposeWindow(IntPtr windowPointer);
		#endregion

		public event Action CloseRequested;

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

		private bool disposed;
		private string title;

		public NativeWindow(NativeApp app, float width, float height)
		{
			NativeWindowPointer = CreateWindow
			(
				app.NativeAppPointer,
				width,
				height,
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

		private void OnCloseRequested() => CloseRequested?.Invoke();
    }
}