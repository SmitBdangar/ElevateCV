using Avalonia.Controls;
using Avalonia.VisualTree;
using System.Linq;
using Luminos.Controls;
using Luminos.Core; // For ToColorArray
using Avalonia;

namespace Luminos.Views
{
    public partial class ToolsPanel : UserControl
    {
        private CanvasView? _canvas;

        public ToolsPanel()
        {
            InitializeComponent();

            BrushSizeSlider.ValueChanged += (_, _) =>
            {
                if (_canvas != null)
                    _canvas.BrushRadius = (float)BrushSizeSlider.Value;
            };

            OpacitySlider.ValueChanged += (_, _) =>
            {
                if (_canvas != null)
                    _canvas.BrushOpacity = (float)OpacitySlider.Value;
            };

            HexGrid.ColorSelected += c =>
            {
                if (_canvas != null)
                    _canvas.ActiveColor = (uint)((c.A << 24) | (c.R << 16) | (c.G << 8) | c.B);
            };
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            _canvas = this.FindAncestorOfType<Window>()?
                .GetVisualDescendants().OfType<CanvasView>().FirstOrDefault();

            HexGrid.Palette = new[]
            {
                new[] { "#FFFFFFFF","#FFEEEEEE","#FFCCCCCC","#FF999999","#FF666666","#FF333333","#FF000000" }.ToColorArray(),
                new[] { "#FFFFE5E5","#FFFFB3B3","#FFFF8080","#FFFF4D4D","#FFFF1A1A","#FFE60000","#FF990000" }.ToColorArray(),
                new[] { "#FFFFF2E0","#FFFFD8B0","#FFFFBB80","#FFFF9F50","#FFFF8220","#FFE66000","#FF993D00" }.ToColorArray(),
                new[] { "#FFFFFFE0","#FFFFFFB3","#FFFFFF80","#FFFFFF4D","#FFFFFF1A","#FFE6E600","#FF999900" }.ToColorArray(),
            };
        }
    }
}
