using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace Luminos.Controls
{
    public partial class ColorWheel : UserControl
    {
        public event Action<Color>? ColorChanged;
        private WriteableBitmap? _wheelBitmap;

        public ColorWheel()
        {
            InitializeComponent();
            this.AttachedToVisualTree += (_, __) => GenerateWheel();
            this.PointerPressed += OnPointer;
        }

        private void GenerateWheel()
        {
            int size = (int)Math.Min(Bounds.Width, Bounds.Height);
            if (size <= 0) return;

            _wheelBitmap = new WriteableBitmap(
                new PixelSize(size, size),
                new Vector(96, 96),
                PixelFormat.Bgra8888,
                AlphaFormat.Unpremul);

            using var buf = _wheelBitmap.Lock();
            unsafe
            {
                uint* pixels = (uint*)buf.Address;
                int radius = size / 2;
                int cx = radius;
                int cy = radius;

                for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    int dx = x - cx;
                    int dy = y - cy;
                    double dist = Math.Sqrt(dx * dx + dy * dy);

                    if (dist > radius) { pixels[y * size + x] = 0; continue; }

                    double angle = (Math.Atan2(dy, dx) + Math.PI) / (2 * Math.PI);
                    double sat = dist / radius;

                    Color c = FromHSV(angle * 360, sat, 1);
                    pixels[y * size + x] = ToUint(c);
                }
            }

            WheelCanvas.Width = size;
            WheelCanvas.Height = size;
            WheelCanvas.Background = new ImageBrush(_wheelBitmap);
        }

        private void OnPointer(object? sender, PointerEventArgs e)
        {
            var p = e.GetPosition(this);
            int size = (int)Math.Min(Bounds.Width, Bounds.Height);

            double cx = size / 2;
            double cy = size / 2;
            double dx = p.X - cx;
            double dy = p.Y - cy;

            double dist = Math.Sqrt(dx * dx + dy * dy);
            if (dist > cx) return;

            double hue = (Math.Atan2(dy, dx) + Math.PI) / (2 * Math.PI) * 360;
            double sat = dist / cx;

            var color = FromHSV(hue, sat, 1);
            ColorChanged?.Invoke(color);
        }

        private static uint ToUint(Color c) =>
            ((uint)c.A << 24) | ((uint)c.R << 16) | ((uint)c.G << 8) | c.B;

        public static Color FromHSV(double h, double s, double v)
{
    h = (h % 360 + 360) % 360; // Normalize
    double C = v * s;
    double X = C * (1 - Math.Abs((h / 60 % 2) - 1));
    double m = v - C;

    double r = 0, g = 0, b = 0;

    if (h < 60) { r = C; g = X; b = 0; }
    else if (h < 120) { r = X; g = C; b = 0; }
    else if (h < 180) { r = 0; g = C; b = X; }
    else if (h < 240) { r = 0; g = X; b = C; }
    else if (h < 300) { r = X; g = 0; b = C; }
    else { r = C; g = 0; b = X; }

    return Color.FromRgb(
        (byte)((r + m) * 255),
        (byte)((g + m) * 255),
        (byte)((b + m) * 255));
}

    }
}
