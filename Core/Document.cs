using Avalonia.Media;

namespace Luminos.Core
{
    public class Document
    {
        public int Width { get; }
        public int Height { get; }

        // Pixel buffer (ARGB format stored as uint per pixel)
        public uint[] Pixels { get; }

        public Document(int width, int height)
        {
            Width = width;
            Height = height;

            Pixels = new uint[width * height];

            // Fill with white background
            for (int i = 0; i < Pixels.Length; i++)
                Pixels[i] = 0xFFFFFFFF; // White
        }
    }
}
