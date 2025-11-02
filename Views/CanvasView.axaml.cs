using System;
using System.Collections.Generic;
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
        private readonly List<Layer> _layers = new();
        private Layer _activeLayer => _layers[0];

        private readonly BrushEngine _brushEngine = new();
        private readonly Renderer _renderer = new();
        private WriteableBitmap _canvasBitmap;
        private Image? _canvasImage;
        private bool _isDrawing = false;

        // ✅ NEW SIMPLE UNDO/REDO SYSTEM
        private readonly Stack<uint[]> _undoStack = new Stack<uint[]>();
        private readonly Stack<uint[]> _redoStack = new Stack<uint[]>();
        private const int MAX_HISTORY = 30;

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
        /// Converts screen pointer position to bitmap pixel coordinates
        /// </summary>
        private Point? GetBitmapCoordinates(PointerEventArgs e)
        {
            if (_canvasImage?.Bounds == null || _canvasBitmap == null)
                return null;

            var pos = e.GetPosition(_canvasImage);
            var imageBounds = _canvasImage.Bounds;
            var bitmapWidth = _canvasBitmap.PixelSize.Width;
            var bitmapHeight = _canvasBitmap.PixelSize.Height;

            double offsetX = (imageBounds.Width - bitmapWidth) / 2.0;
            double offsetY = (imageBounds.Height - bitmapHeight) / 2.0;

            double bitmapX = pos.X - offsetX;
            double bitmapY = pos.Y - offsetY;

            if (bitmapX < 0 || bitmapY < 0 || 
                bitmapX >= bitmapWidth || bitmapY >= bitmapHeight)
            {
                return null;
            }

            return new Point(bitmapX, bitmapY);
        }

        private void CanvasView_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (_canvasImage == null) return;

            var p = e.GetCurrentPoint(_canvasImage);
            if (!p.Properties.IsLeftButtonPressed) return;

            var coords = GetBitmapCoordinates(e);
            if (coords == null) return;

            _isDrawing = true;

            // ✅ SAVE STATE BEFORE DRAWING (full layer snapshot)
            SaveStateForUndo();

            DrawAtPoint(coords.Value.X, coords.Value.Y);

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

            RedrawCanvas();
        }

        private void RedrawCanvas()
        {
            LayerCompositor.Composite(_document, _layers);
            _renderer.Render(_document, _canvasBitmap);

            if (_canvasImage != null)
            {
                var temp = _canvasImage.Source;
                _canvasImage.Source = null;
                _canvasImage.Source = temp;
                
                _canvasImage.InvalidateVisual();
                this.InvalidateVisual();
            }
        }

        // ✅ NEW UNDO/REDO IMPLEMENTATION

        /// <summary>
        /// Save current layer state to undo stack
        /// </summary>
        private void SaveStateForUndo()
        {
            try
            {
                // Clone the current layer pixels
                uint[] snapshot = new uint[_activeLayer.Width * _activeLayer.Height];
                Array.Copy(_activeLayer.GetPixels(), snapshot, snapshot.Length);

                _undoStack.Push(snapshot);

                // Clear redo stack (new action invalidates redo history)
                _redoStack.Clear();

                // Limit history size
                if (_undoStack.Count > MAX_HISTORY)
                {
                    var tempList = new List<uint[]>(_undoStack);
                    _undoStack.Clear();
                    for (int i = 0; i < MAX_HISTORY; i++)
                    {
                        _undoStack.Push(tempList[i]);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"✅ Saved undo state. Stack size: {_undoStack.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Failed to save undo state: {ex.Message}");
            }
        }

        /// <summary>
        /// Undo last action
        /// </summary>
        public void Undo()
        {
            if (_undoStack.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Nothing to undo");
                return;
            }

            try
            {
                // Save current state to redo stack
                uint[] currentState = new uint[_activeLayer.Width * _activeLayer.Height];
                Array.Copy(_activeLayer.GetPixels(), currentState, currentState.Length);
                _redoStack.Push(currentState);

                // Restore previous state
                uint[] previousState = _undoStack.Pop();
                Array.Copy(previousState, _activeLayer.GetPixels(), previousState.Length);

                RedrawCanvas();

                System.Diagnostics.Debug.WriteLine($"✅ Undo successful. Undo stack: {_undoStack.Count}, Redo stack: {_redoStack.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Undo failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Redo last undone action
        /// </summary>
        public void Redo()
        {
            if (_redoStack.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Nothing to redo");
                return;
            }

            try
            {
                // Save current state to undo stack
                uint[] currentState = new uint[_activeLayer.Width * _activeLayer.Height];
                Array.Copy(_activeLayer.GetPixels(), currentState, currentState.Length);
                _undoStack.Push(currentState);

                // Restore redo state
                uint[] redoState = _redoStack.Pop();
                Array.Copy(redoState, _activeLayer.GetPixels(), redoState.Length);

                RedrawCanvas();

                System.Diagnostics.Debug.WriteLine($"✅ Redo successful. Undo stack: {_undoStack.Count}, Redo stack: {_redoStack.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Redo failed: {ex.Message}");
            }
        }
    }
}