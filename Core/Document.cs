namespace Luminos.Core
{
    public class Document
    {
        public int Width { get; }
        public int Height { get; }
        public uint[] Pixels { get; }

        public Document(int width, int height)
        {
            Width = width;
            Height = height;
            Pixels = new uint[width * height];
        }
    }
}
