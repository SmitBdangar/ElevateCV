using Avalonia.Media;
using Luminos.Core;

namespace Luminos.Rendering
{
    public class BrushEngine
    {
        public static BrushEngine Instance { get; } = new BrushEngine();

        public int BrushSize { get; set; } = 10;
        public Color BrushColor { get; set; } = Colors.Black;

        private BrushEngine() { }

        public void ApplyBrush(Document document, int x, int y)
        {
            int r = BrushSize;
            int r2 = r * r;

            for (int dy = -r; dy <= r; dy++)
            {
                for (int dx = -r; dx <= r; dx++)
                {
                    if (dx * dx + dy * dy <= r2)
                    {
                        int px = x + dx;
                        int py = y + dy;

                        if (px >= 0 && px < document.Width && py >= 0 && py < document.Height)
                            document.Pixels[py * document.Width + px] = ColorToUint(BrushColor);
                    }
                }
            }
        }

        private static uint ColorToUint(Color c)
        {
            // BGRA order in memory (lowest byte = B)
            return ((uint)c.B) | ((uint)c.G << 8) | ((uint)c.R << 16) | ((uint)c.A << 24);
        }
    }
}
