using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia;
using Avalonia.Media;
using Avalonia.VisualTree;
using System.Linq;

namespace Luminos.Views
{
    public partial class ToolsPanel : UserControl
    {
        private CanvasView? _canvas;

        public ToolsPanel()
        {
            InitializeComponent();

            var sizeSlider = this.FindControl<Slider>("BrushSizeSlider");
            var opacitySlider = this.FindControl<Slider>("OpacitySlider");
            var colorWheel = this.FindControl<Controls.ColorWheel>("ColorWheelControl");

            if (sizeSlider != null)
                sizeSlider.ValueChanged += BrushSetting_ValueChanged;

            if (opacitySlider != null)
                opacitySlider.ValueChanged += BrushSetting_ValueChanged;

            if (colorWheel != null)
                colorWheel.ActiveColorChanged += ColorWheel_ColorChanged;
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            // Find CanvasView by traversing up to Window then searching descendants
            var window = this.FindAncestorOfType<Window>();
            if (window != null)
            {
                _canvas = window.GetVisualDescendants()
                                .OfType<CanvasView>()
                                .FirstOrDefault();
            }
        }

        private void BrushSetting_ValueChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (_canvas == null)
            {
                // Retry finding canvas
                var window = this.FindAncestorOfType<Window>();
                _canvas = window?.GetVisualDescendants().OfType<CanvasView>().FirstOrDefault();
            }

            if (_canvas == null) return;

            if (sender is Slider slider)
            {
                switch (slider.Name)
                {
                    case "BrushSizeSlider":
                        _canvas.BrushRadius = (float)slider.Value;
                        break;

                    case "OpacitySlider":
                        _canvas.BrushOpacity = (float)slider.Value;
                        break;
                }
            }
        }

        private void ColorWheel_ColorChanged(object? sender, Color color)
        {
            if (_canvas == null) return;
            _canvas.ActiveColor = (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | color.B);
        }
    }
}