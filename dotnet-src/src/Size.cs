using System.Runtime.InteropServices;

namespace NativeMacOS
{
    [StructLayout(LayoutKind.Sequential)]
	public struct Size
	{
		public readonly float Width;
		public readonly float Height;

		public Size(float width, float height)
		{
			Width = width;
			Height = height;
		}

		public override string ToString() => $"width: {Width}, height: {Height}";
	}
}