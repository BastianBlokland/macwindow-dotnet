using System;
using System.Runtime.InteropServices;

namespace MacOS
{
	/// <summary>
	/// Native bindings around a MTKView, this a handle to the metal renderer that renders into a window. 
	/// Lifetime matches the lifetime of the window its attached to
	/// </summary>
    public class NativeMetalView
    {
		#region Native bindings
		[DllImport("libmacwindow")] private static extern IntPtr CreateMetalView(IntPtr windowPointer);
		#endregion

		public readonly IntPtr NativeMetalViewPointer;

		public NativeMetalView(NativeWindow window)
		{
			NativeMetalViewPointer = CreateMetalView(window.NativeWindowPointer);
		}
    }
}
