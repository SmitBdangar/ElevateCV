using System;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Input;
using Avalonia;
using Luminos.Core; // This now provides Document, Layer, and StrokeCommand
// REMOVED: using Luminos.Core.Core; // Was incorrect/redundant after namespace fix
using Luminos.Core.Core;
using Luminos.Rendering;
using Avalonia.Platform;
using System.Collections.Generic;
using System.Linq;

namespace Luminos.Views
{
    public partial class CanvasView : UserControl
    {
        private Document _document;
        
        // REFACTOR: Manages all layers
        private readonly List<Layer> _layers = new List<Layer>();
        
        // Convenience property: Assumes the top layer is the one we draw on for the MVP
        private Layer _activeLayer => _layers.Count > 0 ? _layers.First() : throw new InvalidOperationException("Layer list empty.");
        
        private BrushEngine _brushEngine;
        private Renderer _renderer;
        private WriteableBitmap _canvasBitmap;
        private bool _isDrawing = false;

        private HistoryManager _historyManager = new HistoryManager();

        private uint[]? _preStrokePixels;

        // Brush settings (will be wired via ViewModel in the future)
        private uint _activeColor = 0xFFFF0000; // Opaque Red
        private float _brushRadius = 15.0f;
        private float _brushOpacity = 1.0f;

        // Public properties (used by ToolsPanel for simple data passing)
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

        // Expose bitmap for FileHandler (Export)
        public WriteableBitmap CanvasBitmap => _canvasBitmap;

        public CanvasView()
        {
            InitializeComponent();

            const int defaultWidth = 800;
            const int defaultHeight = 600;

            _document = new Document(defaultWidth, defaultHeight);
            
            // REFACTOR: Create and add the initial layer to the list
            _layers.Add(new Layer(defaultWidth, defaultHeight, "Base Layer"));
            
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

            // Hook pointer events
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

                // Capture layer state BEFORE the stroke starts (full snapshot for MVP undo)
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

                // Capture layer state AFTER the stroke is finished (full snapshot for MVP redo)
                uint[] postStrokePixels = new uint[_activeLayer.Width * _activeLayer.Height];
                Array.Copy(_activeLayer.GetPixels(), postStrokePixels, postStrokePixels.Length);

                if (_preStrokePixels != null)
                {
                    // Create and register the Undo/Redo command
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

            // Calculate dynamic alpha based on brush opacity slider
            uint baseAlpha = (_activeColor >> 24) & 0xFF;
            uint newAlpha = (uint)(baseAlpha * _brushOpacity);
            uint dynamicColor = (newAlpha << 24) | (_activeColor & 0x00FFFFFF);

            _brushEngine.ApplyBrush(_activeLayer, docX, docY, dynamicColor, _brushRadius);

            RedrawCanvas();
        }

        /// <summary>
        /// Orchestrates the rendering pipeline: Compose layers -> Render to Bitmap.
        /// </summary>
        private void RedrawCanvas()
        {
            // 1. Composite all layers (via the Compositor) into the Document's final pixel buffer.
            LayerCompositor.Composite(_document, _layers);
            
            // 2. Render the Document's composite buffer to the Avalonia WriteableBitmap.
            _renderer.Render(_document, _canvasBitmap);
        }
    }
}