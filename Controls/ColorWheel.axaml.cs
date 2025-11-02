using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace Luminos.Controls
{
    public partial class ColorWheel : UserControl
    {
        private bool _isDragging;

        private Color _activeColor = Colors.White;
        public Color ActiveColor
        {
            get => _activeColor;
            set
            {
                _activeColor = value;
                InvalidateVisual();
                ActiveColorChanged?.Invoke(this, value);
            }
        }

        public event EventHandler<Color>? ActiveColorChanged;

        public ColorWheel()
{
    InitializeComponent();
    InvalidateVisual(); // ✅ Force initial render
}

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            double size = Math.Min(Bounds.Width, Bounds.Height);
            double radius = size / 2;
            var center = new Point(Bounds.Width / 2, Bounds.Height / 2);

            // Draw circular hue ring
            for (int angle = 0; angle < 360; angle++)
            {
                var color = HsvToColor(angle, 1, 1);
                var pen = new Pen(new SolidColorBrush(color), 6);

                double rad = Math.PI * angle / 180.0;
                var p1 = center + new Vector(Math.Cos(rad) * (radius - 8), Math.Sin(rad) * (radius - 8));
                var p2 = center + new Vector(Math.Cos(rad) * radius, Math.Sin(rad) * radius);

                context.DrawLine(pen, p1, p2);
            }
        }

        private void ColorWheelSurface_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            _isDragging = true;
            UpdateColorFromPointer(e.GetPosition(this));
        }

        private void ColorWheelSurface_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (_isDragging)
                UpdateColorFromPointer(e.GetPosition(this));
        }

        private void ColorWheelSurface_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _isDragging = false;
        }

        private void UpdateColorFromPointer(Point pos)
        {
            var center = new Point(Bounds.Width / 2, Bounds.Height / 2);
            var vector = pos - center;

            double angle = Math.Atan2(vector.Y, vector.X) * (180 / Math.PI);
            if (angle < 0) angle += 360;

            ActiveColor = HsvToColor((float)angle, 1, 1);
        }

        public static uint HsvToArgb(float h, float s, float v, float a = 1.0f)
        {
            h = h % 360.0f;
            if (h < 0) h += 360.0f;
            s = Math.Clamp(s, 0f, 1f);
            v = Math.Clamp(v, 0f, 1f);
            a = Math.Clamp(a, 0f, 1f);

            float c = v * s;
            float x = c * (1 - Math.Abs((h / 60.0f) % 2 - 1));
            float m = v - c;

            float r, g, b;

            if (h < 60) { r = c; g = x; b = 0; }
            else if (h < 120) { r = x; g = c; b = 0; }
            else if (h < 180) { r = 0; g = c; b = x; }
            else if (h < 240) { r = 0; g = x; b = c; }
            else if (h < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            uint A = (uint)(a * 255);
            uint R = (uint)((r + m) * 255);
            uint G = (uint)((g + m) * 255);
            uint B = (uint)((b + m) * 255);

            return (A << 24) | (R << 16) | (G << 8) | B;
        }

        public static (float h, float s, float v, float a) ArgbToHsv(uint argb)
        {
            float alpha = ((argb >> 24) & 0xFF) / 255f;
            float r = ((argb >> 16) & 0xFF) / 255f;
            float g = ((argb >> 8) & 0xFF) / 255f;
            float b = (argb & 0xFF) / 255f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float delta = max - min;

            float h = 0;
            float s = max == 0 ? 0 : delta / max;
            float v = max;

            if (delta != 0)
            {
                if (max == r) h = 60 * (((g - b) / delta) % 6);
                else if (max == g) h = 60 * (((b - r) / delta) + 2);
                else h = 60 * (((r - g) / delta) + 4);

                if (h < 0) h += 360;
            }

            return (h, s, v, alpha);
        }

        public static Color ArgbToColor(uint argb)
        {
            byte a = (byte)((argb >> 24) & 0xFF);
            byte r = (byte)((argb >> 16) & 0xFF);
            byte g = (byte)((argb >> 8) & 0xFF);
            byte b = (byte)(argb & 0xFF);
            return Color.FromArgb(a, r, g, b);
        }

        public static uint ColorToArgb(Color color)
        {
            return (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | color.B);
        }

        public static Color HsvToColor(float h, float s, float v, float a = 1.0f)
        {
            return ArgbToColor(HsvToArgb(h, s, v, a));
        }
    }
}
