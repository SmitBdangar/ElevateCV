using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Luminos.Core;
using Luminos.Rendering;
using System;

namespace Luminos.Views
{
    public partial class CanvasView : UserControl
    {
        private readonly Document _document;
        private WriteableBitmap _bitmap;
        private (int x, int y)? _lastPoint = null;

        private int _cursorX, _cursorY;
        private bool _showCursor = false;

        public CanvasView()
        {
            InitializeComponent();

            _document = new Document(800, 600);

            _bitmap = new WriteableBitmap(
                new PixelSize(_document.Width, _document.Height),
                new Vector(96, 96),
                Avalonia.Platform.PixelFormat.Bgra8888,
                Avalonia.Platform.AlphaFormat.Premul);

            PointerPressed += OnPointerMove;
            PointerMoved += OnPointerMove;
            PointerReleased += (_, __) => _lastPoint = null;
        }

        private void OnPointerMove(object? sender, PointerEventArgs e)
        {
            var point = e.GetPosition(this);
            _cursorX = (int)point.X;
            _cursorY = (int)point.Y;
            _showCursor = true;

            // ✅ Use ActualWidth instead of Bounds.Width!
            double scaleX = _document.Width / Math.Max(1, this.Bounds.Width);
            double scaleY = _document.Height / Math.Max(1, this.Bounds.Height);


            int x = (int)(point.X * scaleX);
            int y = (int)(point.Y * scaleY);

            var props = e.GetCurrentPoint(this).Properties;

            // ✏️ Drawing
            if (props.IsLeftButtonPressed)
            {
                if (_lastPoint is (int lx, int ly))
                    DrawSmoothLine(lx, ly, x, y);
                else
                    BrushEngine.Instance.ApplyBrush(_document, x, y);

                _lastPoint = (x, y);

                UpdateBitmap();
                InvalidateVisual();
            }
            else
            {
                _lastPoint = null;
            }

            InvalidateVisual();
        }

        private unsafe void UpdateBitmap()
        {
            using (var buf = _bitmap.Lock())
                fixed (uint* src = _document.Pixels)
                    Buffer.MemoryCopy(src, (void*)buf.Address,
                        _document.Pixels.Length * 4,
                        _document.Pixels.Length * 4);
        }

        public override void Render(DrawingContext ctx)
        {
            base.Render(ctx);

            // ✅ Draw Canvas
            ctx.DrawImage(_bitmap, new Rect(_bitmap.Size), new Rect(Bounds.Size));

            // ✅ Brush preview circle
            if (_showCursor)
            {
                int size = BrushEngine.Instance.BrushSize;
                var color = BrushEngine.Instance.IsEraser ? Colors.White : BrushEngine.Instance.BrushColor;

                var previewPen = new Pen(new SolidColorBrush(color), 1);

                ctx.DrawEllipse(
                    null,
                    previewPen,
                    new Avalonia.Point(_cursorX, _cursorY),
                    size,
                    size
                );
            }
        }

        private void DrawSmoothLine(int x0, int y0, int x1, int y1)
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
    }
}
