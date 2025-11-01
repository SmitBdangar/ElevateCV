using System;
using Luminos.Core.Core;

namespace Luminos.Core
{
    /// <summary>
    /// Represents a single painting stroke that can be undone and redone.
    /// Uses full pixel snapshot restore.
    /// </summary>
    public class StrokeCommand : ICommand
    {
        private readonly Layer _targetLayer;
        private readonly uint[] _undoPixels;
        private readonly uint[] _redoPixels;
        private bool _isExecuted = false;

        public StrokeCommand(Layer layer, uint[] beforePixels, uint[] afterPixels)
        {
            _targetLayer = layer;
            _undoPixels = beforePixels;
            _redoPixels = afterPixels;
        }

        // === ICommand implementation ===
        public void Execute()
        {
            Redo();
        }

        public void Undo()
        {
            if (_isExecuted)
            {
                ApplyPixels(_undoPixels);
                _isExecuted = false;
            }
        }

        public void Redo()
        {
            if (!_isExecuted)
            {
                ApplyPixels(_redoPixels);
                _isExecuted = true;
            }
        }

        // === Internal pixel restore logic ===
        private void ApplyPixels(uint[] source)
        {
            uint[] target = _targetLayer.GetPixels();
            Array.Copy(source, target, source.Length);
        }
    }
}
