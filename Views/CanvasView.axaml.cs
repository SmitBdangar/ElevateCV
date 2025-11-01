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
        private bool _isDrawing = false;

        private readonly HistoryManager _historyManager = new();

        // Delta region tracking (for undo/redo stroke efficiency)
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

            this.FindControl<Image>("CanvasImage")!.Source = _canvasBitmap;

            PointerPressed += CanvasView_PointerPressed;
            PointerMoved += CanvasView_PointerMoved;
            PointerReleased += CanvasView_PointerReleased;

            RedrawCanvas();
        }

        private void CanvasView_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var image = this.FindControl<Image>("CanvasImage");
            if (image == null) return;

            var p = e.GetCurrentPoint(image); // Get position relative to Image
            if (!p.Properties.IsLeftButtonPressed) return;

            _isDrawing = true;
            _currentDirtyRect = default;
            _preStrokeDeltaPixels = null;

            DrawAtPoint(p.Position.X, p.Position.Y);

            if (!_currentDirtyRect.IsEmpty)
            {
                _preStrokeDeltaPixels = PixelUtils.GetRegionPixels(
                    _activeLayer.GetPixels(), _activeLayer.Width, _currentDirtyRect);
            }

            e.Handled = true;
        }

        private void CanvasView_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (!_isDrawing) return;

            var image = this.FindControl<Image>("CanvasImage");
            if (image == null) return;

            var p = e.GetCurrentPoint(image); // Get position relative to Image
            DrawAtPoint(p.Position.X, p.Position.Y);
            e.Handled = true;
        }

        private void CanvasView_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (!_isDrawing) return;

            // Finalize stroke first
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

            // Reset drawing state LAST
            _isDrawing = false;
            e.Handled = true;
        }

        private void DrawAtPoint(double x, double y)
        {
            int px = (int)Math.Clamp(x, 0, _document.Width - 1);
            int py = (int)Math.Clamp(y, 0, _document.Height - 1);

            uint baseAlpha = (_activeColor >> 24) & 0xFF;
            uint newAlpha = (uint)(baseAlpha * _brushOpacity);
            uint dynamicColor = (newAlpha << 24) | (_activeColor & 0x00FFFFFF);

            _brushEngine.ApplyBrush(_activeLayer, px, py, dynamicColor, _brushRadius);

            int r = (int)Math.Ceiling(_brushRadius);

            // Clamp dirty rect to canvas bounds
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

            // Clear dirty regions after compositing
            foreach (var layer in _layers)
            {
                layer.ClearDirty();
            }
        }

        // === Undo / Redo Exposed to MainWindow ===
        public void Undo()
        {
            _historyManager.Undo();
            RedrawCanvas(); // Add this
        }

        public void Redo()
        {
            _historyManager.Redo();
            RedrawCanvas(); // Add this
        }
    }
}
