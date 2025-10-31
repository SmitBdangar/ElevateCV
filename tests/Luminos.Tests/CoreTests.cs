using Xunit;
using Luminos.Core;
using System;
using System.Linq;

namespace Luminos.Tests
{
    // A dummy command for HistoryManager testing
    public class TestCommand : ICommand
    {
        public bool IsExecuted { get; private set; } = false;
        public int ExecutionCount { get; private set; } = 0;
        
        public void Execute() 
        { 
            IsExecuted = true;
            ExecutionCount++;
        }
        public void Undo() => IsExecuted = false;
        public void Redo() => Execute();
    }

    public class CoreTests
    {
        [Fact]
        public void Document_Initialization_SetsCorrectDimensionsAndBuffer()
        {
            // Arrange
            const int width = 100;
            const int height = 50;

            // Act
            var doc = new Document(width, height);

            // Assert
            Assert.Equal(width, doc.Width);
            Assert.Equal(height, doc.Height);
            Assert.Equal(width * height, doc.GetPixelsRaw().Length);
            // Verify buffer is initialized to 0 (transparent black)
            Assert.True(doc.GetPixelsRaw().All(p => p == 0x00000000)); 
        }

        [Fact]
        public void Layer_Opacity_ClampsToRange()
        {
            // Act
            var layer = new Layer(10, 10);
            
            // Assert: Default is 1.0f
            Assert.Equal(1.0f, layer.Opacity);

            // Assert: Clamps Max
            layer.Opacity = 2.5f;
            Assert.Equal(1.0f, layer.Opacity);

            // Assert: Clamps Min
            layer.Opacity = -0.5f;
            Assert.Equal(0.0f, layer.Opacity);
            
            // Assert: Valid value
            layer.Opacity = 0.5f;
            Assert.Equal(0.5f, layer.Opacity);
        }

        [Fact]
        public void HistoryManager_DoUndoRedo_WorksCorrectly()
        {
            // Arrange
            var history = new HistoryManager();
            var cmd1 = new TestCommand();
            var cmd2 = new TestCommand();
            
            // 1. Execute cmd1
            history.Do(cmd1); 
            Assert.True(cmd1.IsExecuted);
            Assert.Equal(1, cmd1.ExecutionCount);

            // 2. Undo cmd1
            history.Undo();
            Assert.False(cmd1.IsExecuted);
            
            // 3. Redo cmd1
            history.Redo();
            Assert.True(cmd1.IsExecuted);
            Assert.Equal(2, cmd1.ExecutionCount); // Redo re-executes

            // 4. Execute cmd2 (Redo stack should clear)
            history.Do(cmd2);
            Assert.True(cmd2.IsExecuted);
            Assert.Equal(1, cmd2.ExecutionCount);
            
            // 5. Redo should fail (stack is empty)
            history.Redo();
            Assert.True(cmd2.IsExecuted); // State unchanged
        }
    }
}