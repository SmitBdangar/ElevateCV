using Avalonia.Media;
using Luminos.Core;
using System;

namespace Luminos.Rendering
{
    public class BrushEngine
    {
        public static BrushEngine Instance { get; } = new BrushEngine();

        public int BrushSize { get; set; } = 12;
        public Color BrushColor { get; set; } = Colors.Black;
        public double BrushOpacity { get; set; } = 1.0;
        public bool IsEraser { get; set; } = false;

        private BrushEngine() { }

        public void ApplyBrush(Document document, int x, int y)
        {
            int r = BrushSize;
            int r2 = r * r;

            for (int dy = -r; dy <= r; dy++)
            for (int dx = -r; dx <= r; dx++)
            {
                if (dx * dx + dy * dy <= r2)
                {
                    int px = x + dx;
                    int py = y + dy;

                    if (px >= 0 && px < document.Width && py >= 0 && py < document.Height)
                    {
                        int index = py * document.Width + px;
                        uint src = document.Pixels[index];
                        uint blended = IsEraser
                            ? ErasePixel(src, BrushOpacity)
                            : BlendPixel(src, BrushColor, BrushOpacity);

                        document.Pixels[index] = blended;
                    }
                }
            }
        }

        private static uint BlendPixel(uint dst, Color c, double opacity)
        {
            byte db = (byte)(dst & 255);
            byte dg = (byte)((dst >> 8) & 255);
            byte dr = (byte)((dst >> 16) & 255);
            byte da = (byte)((dst >> 24) & 255);

            byte sr = c.R;
            byte sg = c.G;
            byte sb = c.B;
            byte sa = (byte)(c.A * opacity);

            double a = sa / 255.0;

            byte r = (byte)(dr * (1 - a) + sr * a);
            byte g = (byte)(dg * (1 - a) + sg * a);
            byte b = (byte)(db * (1 - a) + sb * a);
            byte aOut = (byte)Math.Min(255, da + sa * (1 - da / 255.0));

            return (uint)(aOut << 24 | r << 16 | g << 8 | b);
        }

        private static uint ErasePixel(uint dst, double opacity)
        {
            byte db = (byte)(dst & 255);
            byte dg = (byte)((dst >> 8) & 255);
            byte dr = (byte)((dst >> 16) & 255);
            byte da = (byte)((dst >> 24) & 255);

            byte aOut = (byte)(da * (1 - opacity));
            return (uint)(aOut << 24 | dr << 16 | dg << 8 | db);
        }
    }
}
