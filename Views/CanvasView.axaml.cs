using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Luminos.Core;
using Luminos.Rendering;
using System;
using Avalonia.Platform;

namespace Luminos.Views
{
    public partial class CanvasView : UserControl
    {
        private readonly Document _document;
        private WriteableBitmap _bitmap;
        private (int x, int y)? _lastPoint = null;

        public CanvasView()
        {
            InitializeComponent();

            _document = new Document(1200, 800); // big canvas

            _bitmap = new WriteableBitmap(
                new PixelSize(_document.Width, _document.Height),
                new Vector(96, 96),
                PixelFormat.Bgra8888,
                AlphaFormat.Premul);

            PointerPressed += OnPointer;
            PointerMoved += OnPointer;
            PointerReleased += (_, __) => _lastPoint = null;
        }

        private void OnPointer(object? sender, PointerEventArgs e)
        {
            var p = e.GetPosition(this);

            double scaleX = _document.Width / Bounds.Width;
            double scaleY = _document.Height / Bounds.Height;

            int x = (int)(p.X * scaleX);
            int y = (int)(p.Y * scaleY);

            var props = e.GetCurrentPoint(this).Properties;

            if (props.IsLeftButtonPressed)
            {
                if (_lastPoint is (int lx, int ly))
                    DrawLine(lx, ly, x, y);
                else
                    BrushEngine.Instance.ApplyBrush(_document, x, y);

                _lastPoint = (x, y);
                UpdateBitmap();
                InvalidateVisual();
            }
        }

        private void DrawLine(int x0, int y0, int x1, int y1)
        {
            int dx = Math.Abs(x1 - x0), dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                BrushEngine.Instance.ApplyBrush(_document, x0, y0);

                if (x0 == x1 && y0 == y1) break;
                int e2 = err * 2;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
        }

        private unsafe void UpdateBitmap()
        {
            using var buf = _bitmap.Lock();
            fixed (uint* src = _document.Pixels)
            {
                Buffer.MemoryCopy(src, (void*)buf.Address,
                    _document.Pixels.Length * 4,
                    _document.Pixels.Length * 4);
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            // ✅ Draw image to fill visible space
            context.DrawImage(_bitmap,
                new Rect(_bitmap.Size),
                new Rect(Bounds.Size));
        }
    }
}
