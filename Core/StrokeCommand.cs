using System;

namespace Luminos.Core
{
    /// <summary>
    /// Represents a single painting stroke that can be undone and redone.
    /// Captures the pixel state of the affected layer before and after the stroke.
    /// </summary>
    public class StrokeCommand : ICommand
    {
        private readonly Layer _targetLayer;
        private readonly uint[] _undoPixels; // Snapshot of layer pixels BEFORE stroke
        private readonly uint[] _redoPixels; // Snapshot of layer pixels AFTER stroke
        private bool _isExecuted = false;

        /// <summary>
        /// Creates a StrokeCommand with the required pixel data.
        /// </summary>
        /// <param name="layer">The layer that was modified.</param>
        /// <param name="beforePixels">A copy of the layer's pixel array before the stroke.</param>
        /// <param name="afterPixels">A copy of the layer's pixel array after the stroke.</param>
        public StrokeCommand(Layer layer, uint[] beforePixels, uint[] afterPixels)
        {
            _targetLayer = layer;
            _undoPixels = beforePixels;
            _redoPixels = afterPixels;
        }

        /// <summary>
        /// Applies the "after" state to the layer. This is called by HistoryManager.Do()
        /// and HistoryManager.Redo().
        /// </summary>
        public void Execute()
        {
            if (!_isExecuted)
            {
                ApplyPixels(_redoPixels);
                _isExecuted = true;
            }
        }

        /// <summary>
        /// Reverts the layer to the "before" state. This is called by HistoryManager.Undo().
        /// </summary>
        public void Undo()
        {
            if (_isExecuted)
            {
                ApplyPixels(_undoPixels);
                _isExecuted = false;
            }
        }

        /// <summary>
        /// Re-applies the "after" state. This is called by HistoryManager.Redo().
        /// Note: For this simple command, Execute and Redo share the same logic.
        /// </summary>
        public void Redo()
        {
            Execute();
        }

        /// <summary>
        /// Utility to copy a pixel array to the layer's internal buffer.
        /// </summary>
        private void ApplyPixels(uint[] source)
        {
            // Get the layer's internal pixel buffer
            uint[] target = _targetLayer.GetPixels();
            
            // Fast copy the entire array
            Array.Copy(source, target, source.Length);

            // NOTE: For Phase 2 (Performance), this should be changed to use the 
            [cite_start] ;// delta approach (Rect + pixels) to only copy the affected region. 
        }
    }
}