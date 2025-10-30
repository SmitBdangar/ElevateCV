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

        private bool _showCursor = false;
        private int _cursorX, _cursorY;

        private readonly Document _document;
        private BrushEngine BrushEngine => BrushEngine.Instance;

        private WriteableBitmap _bitmap;
        private (int x, int y)? _lastPoint = null;

        public CanvasView()
        {
            InitializeComponent();

            _document = new Document(800, 600);

            _bitmap = new WriteableBitmap(
    new PixelSize(_document.Width, _document.Height),
    new Vector(96, 96),
    Avalonia.Platform.PixelFormat.Bgra8888,
    Avalonia.Platform.AlphaFormat.Premul // or Premul if you expect premultiplied alpha
);


            PointerPressed += OnPointerMoved;
            PointerMoved += OnPointerMoved;
            PointerReleased += (_, __) => _lastPoint = null;
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            var point = e.GetPosition(this);
            _cursorX = (int)point.X;
            _cursorY = (int)point.Y;
            _showCursor = true;

            var props = e.GetCurrentPoint(this).Properties;

            // 🎯 Eyedropper (Right Click)
            if (props.IsRightButtonPressed)
            {
                PickColor(_cursorX, _cursorY);
                InvalidateVisual();
                return;
            }

            // ✏️ Normal Brush (Left Click)
            if (props.IsLeftButtonPressed)
            {
                int x = _cursorX;
                int y = _cursorY;

                if (_lastPoint is (int lx, int ly))
                    BrushEngine.Instance.ApplyBrush(_document, x, y);
                else
                    BrushEngine.Instance.ApplyBrush(_document, x, y);

                _lastPoint = (x, y);
                UpdateBitmap();
            }
            else
            {
                _lastPoint = null;
            }

            InvalidateVisual();

        }

        private unsafe void UpdateBitmap()
        {
            using (var buffer = _bitmap.Lock())
            {
                fixed (uint* src = _document.Pixels)
                {
                    Buffer.MemoryCopy(src, (void*)buffer.Address,
                        _document.Pixels.Length * 4,
                        _document.Pixels.Length * 4);
                }
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            // Draw the image
            context.DrawImage(_bitmap, new Rect(_bitmap.Size), new Rect(Bounds.Size));

            // Draw the brush preview
            if (_showCursor)
            {
                int size = BrushEngine.Instance.BrushSize;
                var color = BrushEngine.Instance.BrushColor;

                var previewPen = new Pen(new SolidColorBrush(color), 1);

                context.DrawEllipse(
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
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                BrushEngine.Instance.ApplyBrush(_document, x0, y0);

                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = err * 2;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
        }

       private void PickColor(int x, int y)
{
    if (x < 0 || x >= _document.Width || y < 0 || y >= _document.Height)
        return;

    uint px = _document.Pixels[y * _document.Width + x];

    byte b = (byte)(px & 0xFF);
    byte g = (byte)((px >> 8) & 0xFF);
    byte r = (byte)((px >> 16) & 0xFF);
    byte a = (byte)((px >> 24) & 0xFF);

    BrushEngine.Instance.BrushColor = Avalonia.Media.Color.FromArgb(a, r, g, b);
}



    }
}
