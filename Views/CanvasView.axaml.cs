using System;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Input;
using Avalonia;
using Luminos.Core;
using Luminos.Core.Core;
using Luminos.Rendering;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform;

namespace Luminos.Views
{
    public partial class CanvasView : UserControl
    {
        private Document _document;
        private readonly List<Layer> _layers = new();
        private Layer _activeLayer => _layers.First();

        private readonly BrushEngine _brushEngine = new();
        private readonly Renderer _renderer = new();
        private WriteableBitmap _canvasBitmap;
        private Image? _canvasImage;
        private bool _isDrawing = false;

        private readonly HistoryManager _historyManager = new();

        private uint[]? _preStrokeDeltaPixels;
        private IntRect _currentDirtyRect = default;

        private uint _activeColor = 0xFFFF0000;
        private float _brushRadius = 15f;
        private float _brushOpacity = 1f;

        public uint ActiveColor { get => _activeColor; set => _activeColor = value; }
        public float BrushRadius { get => _brushRadius; set => _brushRadius = value; }
        public float BrushOpacity { get => _brushOpacity; set => _brushOpacity = value; }
        public WriteableBitmap CanvasBitmap => _canvasBitmap;

        public CanvasView()
        {
            InitializeComponent();

            const int W = 800, H = 600;
            _document = new Document(W, H);
            _layers.Add(new Layer(W, H, "Base Layer"));

            _canvasBitmap = new WriteableBitmap(
                new PixelSize(W, H),
                new Vector(96, 96),
                PixelFormat.Bgra8888,
                AlphaFormat.Premul);

            _canvasImage = this.FindControl<Image>("CanvasImage");
            if (_canvasImage != null)
            {
                _canvasImage.Source = _canvasBitmap;
            }

            PointerPressed += CanvasView_PointerPressed;
            PointerMoved += CanvasView_PointerMoved;
            PointerReleased += CanvasView_PointerReleased;

            RedrawCanvas();
        }

        /// <summary>
        /// ✅ Converts screen pointer position to bitmap pixel coordinates
        /// </summary>
        private Point? GetBitmapCoordinates(PointerEventArgs e)
        {
            if (_canvasImage?.Bounds == null || _canvasBitmap == null)
                return null;

            // Get position relative to Image control
            var pos = e.GetPosition(_canvasImage);

            // Get the Image control's actual rendered size
            var imageBounds = _canvasImage.Bounds;
            var imageWidth = imageBounds.Width;
            var imageHeight = imageBounds.Height;

            // Get bitmap dimensions
            var bitmapWidth = _canvasBitmap.PixelSize.Width;
            var bitmapHeight = _canvasBitmap.PixelSize.Height;

            // Since Stretch="None", the bitmap is rendered at its native size
            // and centered within the Image control.
            // Calculate the offset of the bitmap within the Image bounds:
            double offsetX = (imageWidth - bitmapWidth) / 2.0;
            double offsetY = (imageHeight - bitmapHeight) / 2.0;

            // Transform to bitmap coordinates
            double bitmapX = pos.X - offsetX;
            double bitmapY = pos.Y - offsetY;

            // Validate bounds
            if (bitmapX < 0 || bitmapY < 0 || 
                bitmapX >= bitmapWidth || bitmapY >= bitmapHeight)
            {
                return null; // Outside bitmap area
            }

            return new Point(bitmapX, bitmapY);
        }

        private void CanvasView_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (_canvasImage == null) return;

            var p = e.GetCurrentPoint(_canvasImage);
            if (!p.Properties.IsLeftButtonPressed) return;

            var coords = GetBitmapCoordinates(e);
            if (coords == null) return; // Click outside bitmap

            _isDrawing = true;
            _currentDirtyRect = default;
            _preStrokeDeltaPixels = null;

            DrawAtPoint(coords.Value.X, coords.Value.Y);

            if (!_currentDirtyRect.IsEmpty)
            {
                _preStrokeDeltaPixels = PixelUtils.GetRegionPixels(
                    _activeLayer.GetPixels(), _activeLayer.Width, _currentDirtyRect);
            }

            e.Handled = true;
        }

        private void CanvasView_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (!_isDrawing || _canvasImage == null) return;

            var coords = GetBitmapCoordinates(e);
            if (coords == null) return;

            DrawAtPoint(coords.Value.X, coords.Value.Y);
            e.Handled = true;
        }

        private void CanvasView_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (!_isDrawing) return;

            if (!_currentDirtyRect.IsEmpty && _preStrokeDeltaPixels != null)
            {
                uint[] postPixels = PixelUtils.GetRegionPixels(
                    _activeLayer.GetPixels(), _activeLayer.Width, _currentDirtyRect);

                _historyManager.Do(new StrokeCommand(
                    _activeLayer,
                    _currentDirtyRect,
                    _preStrokeDeltaPixels,
                    postPixels));

                _preStrokeDeltaPixels = null;
                _currentDirtyRect = default;
            }

            _isDrawing = false;
            e.Handled = true;
        }

        private void DrawAtPoint(double x, double y)
        {
            int px = (int)Math.Clamp(x, 0, _document.Width - 1);
            int py = (int)Math.Clamp(y, 0, _document.Height - 1);

            uint newAlpha = (uint)(255 * _brushOpacity);
            uint dynamicColor = (newAlpha << 24) | (_activeColor & 0x00FFFFFF);

            _brushEngine.ApplyBrush(_activeLayer, px, py, dynamicColor, _brushRadius);

            int r = (int)Math.Ceiling(_brushRadius);

            int rectX = Math.Max(0, px - r);
            int rectY = Math.Max(0, py - r);
            int rectW = Math.Min(_document.Width - rectX, r * 2);
            int rectH = Math.Min(_document.Height - rectY, r * 2);

            IntRect brushRect = new(rectX, rectY, rectW, rectH);

            _currentDirtyRect = IntRect.Union(_currentDirtyRect, brushRect);
            _activeLayer.MarkDirty(_currentDirtyRect);

            RedrawCanvas();
        }

        private void RedrawCanvas()
        {
            LayerCompositor.Composite(_document, _layers);
            _renderer.Render(_document, _canvasBitmap);

            foreach (var layer in _layers)
            {
                layer.ClearDirty();
            }

            if (_canvasImage != null)
            {
                // Force UI refresh by detaching/reattaching
                var temp = _canvasImage.Source;
                _canvasImage.Source = null;
                _canvasImage.Source = temp;
                
                _canvasImage.InvalidateVisual();
                this.InvalidateVisual();
            }
        }

        public void Undo()
        {
            _historyManager.Undo();
            RedrawCanvas();
        }

        public void Redo()
        {
            _historyManager.Redo();
            RedrawCanvas();
        }
    }
}