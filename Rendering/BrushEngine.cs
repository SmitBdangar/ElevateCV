using Avalonia.Media;
using Luminos.Core;

namespace Luminos.Rendering
{
    public static class BrushEngine
    {
        private static int _brushSize = 10;

        public static Color BrushColor { get; set; } = Colors.LimeGreen;

        public static void ApplyBrush(Document document, int x, int y)
        {
            for (int dy = -_brushSize; dy <= _brushSize; dy++)
            {
                for (int dx = -_brushSize; dx <= _brushSize; dx++)
                {
                    int px = x + dx;
                    int py = y + dy;

                    if (px >= 0 && px < document.Width && py >= 0 && py < document.Height)
                    {
                        document.Pixels[py * document.Width + px] = ColorToUint(BrushColor);
                    }
                }
            }
        }

        private static uint ColorToUint(Color color)
        {
            return ((uint)color.A << 24) | ((uint)color.R << 16) | ((uint)color.G << 8) | color.B;
        }
    }
}
