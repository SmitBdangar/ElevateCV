using Avalonia.Media;
using Luminos.Core;

namespace Luminos.Rendering
{
    public class BrushEngine
    {
        // ✅ Singleton instance
        public static BrushEngine Instance { get; } = new BrushEngine();

        public int BrushSize { get; set; } = 10;
        public double BrushOpacity { get; set; } = 1.0;
        public Color BrushColor { get; set; } = Colors.LimeGreen;
        public bool IsEraser { get; set; } = false;


        private BrushEngine() { }

        public void ApplyBrush(Document document, int x, int y)
        {
            int radius = BrushSize;
            int r2 = radius * radius;

            uint paintColor = IsEraser
                ? 0xFFFFFFFF
                : ColorToUint(BrushColor, BrushOpacity);

            for (int dy = -radius; dy <= radius; dy++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    if (dx * dx + dy * dy <= r2)
                    {
                        int px = x + dx;
                        int py = y + dy;

                        if (px >= 0 && px < document.Width && py >= 0 && py < document.Height)
                            document.Pixels[py * document.Width + px] = paintColor;
                    }
                }
            }
        }


        private static uint ColorToUint(Color c, double opacity)
        {
            byte a = (byte)(c.A * opacity);
            return ((uint)a << 24) | ((uint)c.R << 16) | ((uint)c.G << 8) | c.B;
        }
    }
}
