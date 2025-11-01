using System;
using Luminos.Core.Core;

namespace Luminos.Core
{
    /// <summary>
    /// Represents a single painting stroke that can be undone and redone
    /// using delta regions (dirty rectangle + pixel delta buffers).
    /// This minimizes memory usage compared to full layer snapshots.
    /// </summary>
    public class StrokeCommand : ICommand
    {
        private readonly Layer _targetLayer;
        private readonly IntRect _dirtyRect;
        private readonly uint[] _undoPixels;
        private readonly uint[] _redoPixels;
        private bool _isExecuted = false;

        public StrokeCommand(Layer layer, IntRect dirtyRect, uint[] beforePixels, uint[] afterPixels)
        {
            _targetLayer = layer;
            _dirtyRect = dirtyRect;
            _undoPixels = beforePixels;
            _redoPixels = afterPixels;
        }

        // ICommand Implementation
        public void Execute() => Redo();

        public void Undo()
        {
            if (_isExecuted)
            {
                ApplyPixels(_undoPixels);
                _isExecuted = false;
                _targetLayer.MarkDirty(_dirtyRect); // << refresh UI
            }
        }

        public void Redo()
        {
            if (!_isExecuted)
            {
                ApplyPixels(_redoPixels);
                _isExecuted = true;
                _targetLayer.MarkDirty(_dirtyRect); // << refresh UI
            }
        }

        /// <summary>
        /// Copies the delta-region pixels into the layer buffer.
        /// Only affects the dirty rectangle.
        /// </summary>
        private void ApplyPixels(uint[] source)
        {
            uint[] target = _targetLayer.GetPixels();
            int sourceIndex = 0;

            for (int y = _dirtyRect.Y; y < _dirtyRect.Y + _dirtyRect.Height; y++)
            {
                int targetStart = y * _targetLayer.Width + _dirtyRect.X;
                Array.Copy(source, sourceIndex, target, targetStart, _dirtyRect.Width);
                sourceIndex += _dirtyRect.Width;
            }
        }
    }
}
