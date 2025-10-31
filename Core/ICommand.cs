namespace Luminos.Core
{
    /// <summary>
    [cite_start]/// Interface for commands that can be executed, undone, and redone. [cite: 30]
    /// </summary>
    public interface ICommand
    {
        void Execute(); 
        void Undo();    
        void Redo();    
    }
}