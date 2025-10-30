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

        public CanvasView()
        {
            InitializeComponent();

            // Create a default 800x600 drawing document
            _document = new Document(800, 600);

            _bitmap = new WriteableBitmap(
                new PixelSize(_document.Width, _document.Height),
                new Vector(96, 96),
                Avalonia.Platform.PixelFormat.Bgra8888
            );

            PointerMoved += OnPointerMoved;
            PointerPressed += OnPointerMoved;
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            var point = e.GetPosition(this);

            // Draw only while left mouse button held
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                BrushEngine.ApplyBrush(_document, (int)point.X, (int)point.Y);
                UpdateBitmap();
                InvalidateVisual();
            }
        }

        private unsafe void UpdateBitmap()
        {
            using (var buffer = _bitmap.Lock())
            {
                fixed (uint* src = _document.Pixels)
                {
                    Buffer.MemoryCopy(src, (void*)buffer.Address, _document.Pixels.Length * 4, _document.Pixels.Length * 4);
                }
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            context.DrawImage(_bitmap, new Rect(_bitmap.Size), new Rect(Bounds.Size));

        }
    }
}
