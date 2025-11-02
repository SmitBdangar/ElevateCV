using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;

namespace Luminos.Controls
{
    public sealed class HexColorGrid : Control
    {
        public int Rows { get; set; } = 6;
        public int Columns { get; set; } = 6;
        public double CellSize { get; set; } = 22.0;

        public event Action<Color>? ColorSelected;

        private static readonly Color[] Palette =
        {
            Colors.White, Colors.LightGray, Colors.Gray, Colors.Black,
            Colors.Red, Colors.Orange, Colors.Yellow, Colors.Green,
            Colors.Cyan, Colors.Blue, Colors.Indigo, Colors.Violet,
            Colors.Pink, Colors.Brown, Colors.Gold, Colors.Lime,
            Colors.Teal, Colors.SkyBlue, Colors.Purple, Colors.Magenta,
            Colors.Coral, Colors.Salmon, Colors.Olive, Colors.Navy,
            Colors.DarkRed, Colors.DarkGreen, Colors.DarkBlue, Colors.Silver,
            Colors.Aqua, Colors.Tan, Colors.Khaki, Colors.Maroon,
            Colors.Chartreuse, Colors.Tomato, Colors.Sienna, Colors.SlateGray
        };

        private Point[] HexPoints(double x, double y, double size)
        {
            double w = size;
            double h = size * 0.866; // sin(60°)

            return new[]
            {
                new Point(x + w * 0.5, y),
                new Point(x + w, y + h * 0.5),
                new Point(x + w * 0.5, y + h),
                new Point(x, y + h * 0.5),
            };
        }

        public override void Render(DrawingContext context)
        {
            var index = 0;
            double w = CellSize;
            double h = CellSize * 0.866;

            for (int r = 0; r < Rows; r++)
            {
                double y = r * h;
                double offset = (r % 2) * (w * 0.5);

                for (int c = 0; c < Columns; c++)
                {
                    if (index >= Palette.Length)
                        return;

                    double x = c * w + offset;
                    var color = Palette[index++];

                    var pts = HexPoints(x, y, w);
                    var geo = new PolylineGeometry(pts, true);

                    context.DrawGeometry(new SolidColorBrush(color), null, geo);
                }
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            var p = e.GetPosition(this);

            double w = CellSize;
            double h = CellSize * 0.866;

            int r = (int)(p.Y / h);
            double offset = (r % 2) * (w * 0.5);
            int c = (int)((p.X - offset) / w);

            int index = r * Columns + c;

            if (index >= 0 && index < Palette.Length)
                ColorSelected?.Invoke(Palette[index]);
        }
    }
}
