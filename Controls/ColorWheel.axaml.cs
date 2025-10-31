using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;   // ✅ PixelFormat, PixelSize, AlphaFormat
using System;



namespace Luminos.Controls
{
    public partial class ColorWheel : UserControl
    {
        public event Action<Color>? ColorChanged;

        private WriteableBitmap? _bitmap;
        private double _brightness = 1.0;

        public ColorWheel()
        {
            InitializeComponent();

            CreateWheel();

            BrightnessSlider.PropertyChanged += (_, e) =>
            {
                if (e.Property.Name == "Value")
                {
                    _brightness = BrightnessSlider.Value;
                    ColorChanged?.Invoke(GetColor(_lastX, _lastY));
                }
            };

            PointerPressed += OnPointer;
            PointerMoved += OnPointer;
        }

        private int _lastX, _lastY;

        private void OnPointer(object? sender, PointerEventArgs e)
        {
            var p = e.GetPosition(WheelImage);
            _lastX = (int)p.X;
            _lastY = (int)p.Y;
            ColorChanged?.Invoke(GetColor(_lastX, _lastY));
        }

        /// Draws hue+saturation wheel into bitmap
        private void CreateWheel()
        {
            int size = 160;
            _bitmap = new WriteableBitmap(new PixelSize(size, size), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Unpremul);
            WheelImage.Source = _bitmap;

            using var buf = _bitmap.Lock();
            unsafe
            {
                uint* data = (uint*)buf.Address;
                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                    {
                        double dx = (x - size / 2.0) / (size / 2.0);
                        double dy = (y - size / 2.0) / (size / 2.0);
                        double dist = Math.Sqrt(dx * dx + dy * dy);
                        if (dist <= 1)
                        {
                            double angle = Math.Atan2(dy, dx);
                            double hue = (angle / Math.PI + 1) * 180;
                            double sat = dist;
                            var c = FromHsv(hue, sat, 1.0);
                            data[y * size + x] = ((uint)c.A << 24) | ((uint)c.R << 16) | ((uint)c.G << 8) | c.B;
                        }
                        else data[y * size + x] = 0;
                    }
            }
        }

        private Color GetColor(int x, int y)
        {
            int size = 160;
            double dx = (x - size / 2.0) / (size / 2.0);
            double dy = (y - size / 2.0) / (size / 2.0);
            double dist = Math.Sqrt(dx * dx + dy * dy);
            if (dist > 1) return Colors.Transparent;

            double angle = Math.Atan2(dy, dx);
            double hue = (angle / Math.PI + 1) * 180;
            double sat = dist;
            return FromHsv(hue, sat, _brightness);
        }

        private static Color FromHsv(double h, double s, double v)
        {
            double c = v * s;
            double x = c * (1 - Math.Abs((h / 60 % 2) - 1));
            double m = v - c;

            double r, g, b;

            if (h < 60)
                (r, g, b) = (c, x, 0);
            else if (h < 120)
                (r, g, b) = (x, c, 0);
            else if (h < 180)
                (r, g, b) = (0, c, x);
            else if (h < 240)
                (r, g, b) = (0, x, c);
            else if (h < 300)
                (r, g, b) = (x, 0, c);
            else
                (r, g, b) = (c, 0, x);


            return Color.FromArgb(255,
                (byte)((r + m) * 255),
                (byte)((g + m) * 255),
                (byte)((b + m) * 255));
        }
    }
}
