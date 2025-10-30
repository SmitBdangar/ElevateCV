namespace Luminos.Core
{
    public class Document
    {
        public int Width { get; }
        public int Height { get; }

        // Raw pixel buffer (BGRA32 stored in uint)
        public uint[] Pixels { get; }

        public Document(int width, int height)
        {
            Width = width;
            Height = height;
            Pixels = new uint[width * height];

       for (int i = 0; i < Pixels.Length; i++)
    Pixels[i] = 0xFFFFFFFFu; // 0xAARRGGBB not used here — but with BGRA storage it's 0x?? actually A in high byte
// with BGRA storage that represents A=0xFF, R=0xFF, G=0xFF, B=0xFF => white

        }
    }
}
