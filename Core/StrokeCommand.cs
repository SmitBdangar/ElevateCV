using System;

namespace Luminos.Core
{
    public class StrokeCommand : ICommand
    {
        private readonly Layer _targetLayer;
        private readonly uint[] _undoPixels; // Snapshot of layer pixels BEFORE stroke
        private readonly uint[] _redoPixels; // Snapshot of layer pixels AFTER stroke
        private bool _isExecuted = false;

        public StrokeCommand(Layer layer, uint[] beforePixels, uint[] afterPixels)
        {
            _targetLayer = layer;
            _undoPixels = beforePixels;
            _redoPixels = afterPixels;
        }
        public void Execute()
        {
            if (!_isExecuted)
            {
                ApplyPixels(_redoPixels);
                _isExecuted = true;
            }
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
            Execute();
        }

        private void ApplyPixels(uint[] source)
        {
            uint[] target = _targetLayer.GetPixels();
            Array.Copy(source, target, source.Length);

        }
    }
}