using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Luminos.Views
{
    public partial class ToolsPanel : UserControl
    {
        public ToolsPanel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            
            // FUTURE: Wire up slider value changes to update the CanvasViewModel/BrushEngine settings.
        }
    }
}
