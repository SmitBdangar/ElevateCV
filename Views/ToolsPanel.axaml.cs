using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using Avalonia.Controls.Primitives;
using Luminos.Controls;
using System.Linq;

namespace Luminos.Views
{
    public partial class ToolsPanel : UserControl
    {
        private CanvasView? _canvas;

        public ToolsPanel()
        {
            InitializeComponent();

            BrushSizeSlider.ValueChanged += BrushChanged;
            OpacitySlider.ValueChanged += BrushChanged;

            ColorGrid.ColorSelected += OnColorPicked;
            ColorPad.ColorChanged += OnColorPicked;
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            var window = this.FindAncestorOfType<Window>();
            if (window == null) return;

            _canvas = window.GetVisualDescendants()
                            .OfType<CanvasView>()
                            .FirstOrDefault();
        }

        private void OnColorPicked(Color color)
        {
            if (_canvas == null)
            {
                var window = this.FindAncestorOfType<Window>();
                _canvas = window?.GetVisualDescendants().OfType<CanvasView>().FirstOrDefault();
                if (_canvas == null) return;
            }

            _canvas.ActiveColor = (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | color.B);

            CurrentColorPreview.Background = new SolidColorBrush(color);
        }

        private void BrushChanged(object? sender, RangeBaseValueChangedEventArgs e)
        {
            if (_canvas == null)
            {
                var window = this.FindAncestorOfType<Window>();
                _canvas = window?.GetVisualDescendants().OfType<CanvasView>().FirstOrDefault();
                if (_canvas == null) return;
            }

            _canvas.BrushRadius = (float)BrushSizeSlider.Value;
            _canvas.BrushOpacity = (float)OpacitySlider.Value;
        }
    }
}
