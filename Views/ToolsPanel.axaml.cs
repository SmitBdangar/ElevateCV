using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using Luminos.Controls;

namespace Luminos.Views
{
    public partial class ToolsPanel : UserControl
    {
        private CanvasView? _canvas;

        public ToolsPanel()
        {
            InitializeComponent();

            // Find canvas after UI finishes loading
            this.Loaded += (_, __) =>
            {
                var window = this.GetVisualRoot() as Window;
                if (window != null)
                {
                    _canvas = window.GetVisualDescendants()
                                    .OfType<CanvasView>()
                                    .FirstOrDefault();
                }
            };

            BrushSizeSlider.ValueChanged += (_, __) => UpdateBrushSettings();
            OpacitySlider.ValueChanged += (_, __) => UpdateBrushSettings();

            ColorPicker.ActiveColorChanged += (_, color) =>
            {
                ColorPreview.Background = new SolidColorBrush(color);

                if (_canvas != null)
                {
                    _canvas.ActiveColor = (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | color.B);
                }
            };
        }

        private void UpdateBrushSettings()
        {
            if (_canvas == null) return;

            _canvas.BrushRadius = (float)BrushSizeSlider.Value;
            _canvas.BrushOpacity = (float)OpacitySlider.Value;
        }
    }
}
