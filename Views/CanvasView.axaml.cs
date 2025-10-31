using System;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Input;
using Avalonia;
using Luminos.Core;
using Luminos.Rendering;
using Avalonia.Platform;

namespace Luminos.Views
{
    public partial class CanvasView : UserControl
    {
        private Document _document;
        private Layer _activeLayer;
        private BrushEngine _brushEngine;
        private Renderer _renderer;
        private WriteableBitmap _canvasBitmap;
        private bool _isDrawing = false;

        private HistoryManager _historyManager = new HistoryManager();

        private uint[]? _preStrokePixels;

        // Brush settings
        private uint _activeColor = 0xFFFF0000; // ARGB Red
        private float _brushRadius = 15.0f;
        private float _brushOpacity = 1.0f;

        // Public properties (used by ToolsPanel)
        public uint ActiveColor
        {
            get => _activeColor;
            set => _activeColor = value;
        }

        public float BrushRadius
        {
            get => _brushRadius;
            set => _brushRadius = value;
        }

        public float BrushOpacity
        {
            get => _brushOpacity;
            set => _brushOpacity = value;
        }

        // Expose bitmap for Export
        public WriteableBitmap CanvasBitmap => _canvasBitmap;

        public CanvasView()
        {
            InitializeComponent();

            const int defaultWidth = 800;
            const int defaultHeight = 600;

            _document = new Document(defaultWidth, defaultHeight);
            _activeLayer = new Layer(defaultWidth, defaultHeight, "Base Layer");
            _brushEngine = new BrushEngine();
            _renderer = new Renderer();

            _canvasBitmap = new WriteableBitmap(
                new PixelSize(defaultWidth, defaultHeight),
                new Vector(96, 96),
                PixelFormat.Bgra8888,
                AlphaFormat.Premul);

            var canvasImage = this.FindControl<Image>("CanvasImage");
            if (canvasImage == null)
                throw new InvalidOperationException("CanvasImage control not found in XAML.");
            canvasImage.Source = _canvasBitmap;

            // ✅ Hook pointer events (you were missing this!)
            PointerPressed += CanvasView_PointerPressed;
            PointerMoved += CanvasView_PointerMoved;
            PointerReleased += CanvasView_PointerReleased;

            RedrawCanvas();
        }

        private void CanvasView_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(this);

            if (point.Properties.IsLeftButtonPressed)
            {
                _isDrawing = true;

                _preStrokePixels = new uint[_activeLayer.Width * _activeLayer.Height];
                Array.Copy(_activeLayer.GetPixels(), _preStrokePixels, _preStrokePixels.Length);

                DrawAtPoint(point.Position.X, point.Position.Y);
                e.Handled = true;
            }
        }

        private void CanvasView_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (_isDrawing)
            {
                var point = e.GetCurrentPoint(this);
                DrawAtPoint(point.Position.X, point.Position.Y);
                e.Handled = true;
            }
        }

        private void CanvasView_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (_isDrawing)
            {
                _isDrawing = false;

                uint[] postStrokePixels = new uint[_activeLayer.Width * _activeLayer.Height];
                Array.Copy(_activeLayer.GetPixels(), postStrokePixels, postStrokePixels.Length);

                if (_preStrokePixels != null)
                {
                    var command = new StrokeCommand(_activeLayer, _preStrokePixels, postStrokePixels);
                    _historyManager.Do(command);
                }

                _preStrokePixels = null;
                e.Handled = true;
            }
        }

        private void DrawAtPoint(double x, double y)
        {
            int docX = (int)x;
            int docY = (int)y;

            uint baseAlpha = (_activeColor >> 24) & 0xFF;
            uint newAlpha = (uint)(baseAlpha * _brushOpacity);
            uint dynamicColor = (newAlpha << 24) | (_activeColor & 0x00FFFFFF);

            _brushEngine.ApplyBrush(_activeLayer, docX, docY, dynamicColor, _brushRadius);

            RedrawCanvas();
        }

        private void RedrawCanvas()
        {
            Array.Copy(_activeLayer.GetPixels(), _document.GetPixelsRaw(), _document.Width * _document.Height);
            _renderer.Render(_document, _canvasBitmap);
        }
    }
}
