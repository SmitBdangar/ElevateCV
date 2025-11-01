namespace Luminos.Core
{
    /// <summary>
    /// Represents an integer-based rectangular region (used for dirty tracking).
    /// </summary>
    public struct IntRect
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public IntRect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool IsEmpty => Width <= 0 || Height <= 0;

        /// <summary>
        /// Combines two rectangles into a single bounding box that contains both.
        /// </summary>
        public static IntRect Union(IntRect a, IntRect b)
        {
            if (a.IsEmpty) return b;
            if (b.IsEmpty) return a;

            int x1 = System.Math.Min(a.X, b.X);
            int y1 = System.Math.Min(a.Y, b.Y);
            int x2 = System.Math.Max(a.X + a.Width, b.X + b.Width);
            int y2 = System.Math.Max(a.Y + a.Height, b.Y + b.Height);

            return new IntRect(x1, y1, x2 - x1, y2 - y1);
        }
    }
}