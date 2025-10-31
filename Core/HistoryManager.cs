using System.Collections.Generic;

namespace Luminos.Core
{
    /// <summary>
    [cite_start]/// Manages the command stack for undo/redo functionality using commands/mementos. [cite: 30, 116]
    /// </summary>
    public class HistoryManager
    {
        private readonly Stack<ICommand> _undoStack = new Stack<ICommand>();
        private readonly Stack<ICommand> _redoStack = new Stack<ICommand>();

        /// <summary>
        [cite_start]/// Executes a command and adds it to the undo stack. [cite: 116]
        /// </summary>
        public void Do(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear(); 
        }

        /// <summary>
        [cite_start]/// Undoes the last executed command. [cite: 116]
        /// </summary>
        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
            }
        }

        /// <summary>
        [cite_start]/// Re-executes the last undone command. [cite: 116]
        /// </summary>
        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
                command.Redo();
                _undoStack.Push(command);
            }
        }
    }
}