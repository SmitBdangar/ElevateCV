using System;
using Avalonia.Controls;
using System.ComponentModel;
using Avalonia.Controls.ApplicationLifetimes;


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

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            base.OnClosing(e);
            // your close logic here
        }
    }
}
