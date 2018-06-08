using System.Runtime.InteropServices;

namespace NativeMacOS
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Int2
    {
        public readonly int X;
        public readonly int Y;

        public Int2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Int2 operator +(Int2 left, Int2 right) => new Int2(left.X + right.X, left.Y + right.Y);

        public static Int2 operator -(Int2 left, Int2 right) => new Int2(left.X - right.X, left.Y - right.Y);

        public override string ToString() => $"(X: {X}, Y: {Y})";
    }
}