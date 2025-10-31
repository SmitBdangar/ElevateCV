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
        
        // MVP Brush Settings (Hardcoded for Phase 1)
        private uint _activeColor = 0xFFFF0000; // Opaque Red (0xAARRGGBB)
        private float _brushRadius = 15.0f;

        public CanvasView()
        {
            InitializeComponent();
            
            // 1. Initialize Core Components (Using a standard size for MVP canvas)
            const int defaultWidth = 800;
            const int defaultHeight = 600;
            _document = new Document(defaultWidth, defaultHeight);
            _activeLayer = new Layer(defaultWidth, defaultHeight, "Base Layer");
            
            _brushEngine = new BrushEngine();
            _renderer = new Renderer();

            // 2. Initialize Avalonia WriteableBitmap
            _canvasBitmap = new WriteableBitmap(
                new PixelSize(defaultWidth, defaultHeight),
                new Vector(96, 96), // Default DPI
                PixelFormat.Bgra8888, // Common format
                AlphaFormat.Premul // Use premultiplied alpha
            );

            // 3. Set the Image Source
            this.FindControl<Image>("CanvasImage").Source = _canvasBitmap;

            // 4. Initial Render
            RedrawCanvas();
        }

        private void CanvasView_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(this);
            if (point.Properties.IsLeftButtonPressed) // Left-click: paint [cite: 92]
            {
                _isDrawing = true;
                DrawAtPoint(point.Position.X, point.Position.Y);
                e.Handled = true;
            }
             ;// FUTURE: Right-click: pick color [cite: 93]
        }

        private void CanvasView_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (_isDrawing)
            {
                var point = e.GetCurrentPoint(this);
                // FUTURE: Stroke smoothing (Phase 3) [cite: 81]
                DrawAtPoint(point.Position.X, point.Position.Y);
                e.Handled = true;
            }
        }

        private void CanvasView_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _isDrawing = false;
            // FUTURE: When a stroke is finished, create a command and pass it to HistoryManager.Do()
        }

        private void DrawAtPoint(double x, double y)
        {
            // Map screen coordinates to document coordinates (simplified: 1:1 for MVP)
            int docX = (int)x;
            int docY = (int)y;

            // Apply the brush engine logic to the active layer's pixel buffer
            _brushEngine.ApplyBrush(_activeLayer, docX, docY, _activeColor, _brushRadius);
            
            // Re-render the canvas to reflect the change
            RedrawCanvas();
        }

        /// <summary>
        /// Renders the document's active layer pixels to the WriteableBitmap using the Renderer.
        /// </summary>
        private void RedrawCanvas()
        {
            // Update the document's main buffer from the layer (simplified composition for MVP)
            // In a full application, a Layer Composer would run here.
            
            // For MVP, we pass the active layer's pixels to the document buffer for the Renderer to read.
            Array.Copy(_activeLayer.GetPixels(), _document.GetPixelsRaw(), _document.Width * _document.Height);
            
            _renderer.Render(_document, _canvasBitmap);

             ;// Phase 2 Goal: update only dirty rectangle region [cite: 82]
        }
    }
}
