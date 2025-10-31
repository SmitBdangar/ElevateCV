using Avalonia.Controls;
using System.ComponentModel;

namespace Luminos
{
    // Note: The class name corresponds to the namespace 'Luminos' and class 'MainWindow'
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // FUTURE: Override OnKeyDown to handle global shortcuts like Ctrl+Z/Ctrl+Y (Undo/Redo)
        
        protected override void OnClosing(CancelEventArgs e)
        {
            // FUTURE: Implement logic to prompt user to save changes before closing
            base.OnClosing(e);
        }
    }
}
