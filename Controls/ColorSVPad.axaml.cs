using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;   // ← this fixes PixelFormat + AlphaFormat
using System;


namespace Luminos.Controls
{
    public partial class ColorSVPad : UserControl
    {
        public event Action<Color>? ColorChanged;

        private WriteableBitmap? _bitmap;
        private double _hue = 0.0; // 0-360
        private bool _down;

        public ColorSVPad()
        {
            InitializeComponent();
            PointerPressed += OnDown;
            PointerMoved += OnMove;
            PointerReleased += (_, __) => _down = false;

            this.AttachedToVisualTree += (_, __) =>
            {
                GenerateBitmap();
            };
        }

        public void SetHue(double hue)
        {
            _hue = hue;
            GenerateBitmap();
        }

        private void OnDown(object? sender, PointerPressedEventArgs e)
        {
            _down = true;
            PickColor(e);
        }

        private void OnMove(object? sender, PointerEventArgs e)
        {
            if (_down)
                PickColor(e);
        }

        private void PickColor(PointerEventArgs e)
        {
            var p = e.GetPosition(this);

            double s = Math.Clamp(p.X / Bounds.Width, 0, 1);
            double v = Math.Clamp(1 - (p.Y / Bounds.Height), 0, 1);

            var color = FromHsv(_hue, s, v);
            ColorChanged?.Invoke(color);
        }

        private void GenerateBitmap()
        {
            if (Bounds.Width <= 0 || Bounds.Height <= 0) return;

            int w = (int)Bounds.Width;
            int h = (int)Bounds.Height;

            _bitmap = new WriteableBitmap(new PixelSize(w, h), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
            using var fb = _bitmap.Lock();

            unsafe
            {
                uint* ptr = (uint*)fb.Address.ToPointer();
                for (int y = 0; y < h; y++)
                {
                    double v = 1 - (double)y / h;
                    for (int x = 0; x < w; x++)
                    {
                        double s = (double)x / w;
                        var c = FromHsv(_hue, s, v);
                        *ptr++ = (uint)((c.A << 24) | (c.R << 16) | (c.G << 8) | c.B);
                    }
                }
            }

            PadCanvas.Background = new ImageBrush(_bitmap);
        }

        private static Color FromHsv(double h, double s, double v)
        {
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

            return Color.FromArgb(255,
                (byte)((r + m) * 255),
                (byte)((g + m) * 255),
                (byte)((b + m) * 255));
        }
    }
}
