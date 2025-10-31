using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia;
using Avalonia.Media;
using Avalonia.VisualTree;        // For FindAncestorOfType()           // For Color type
using Luminos.Views;              // For CanvasView


namespace Luminos.Views
{
    public partial class ToolsPanel : UserControl
    {
        private CanvasView? _canvas;

        public ToolsPanel()
        {
            InitializeComponent();

            this.FindControl<Slider>("BrushSizeSlider")!.ValueChanged += BrushSetting_ValueChanged;
            this.FindControl<Slider>("OpacitySlider")!.ValueChanged += BrushSetting_ValueChanged;
            this.FindControl<Controls.ColorWheel>("ColorWheelControl")!.PropertyChanged += ColorWheel_PropertyChanged;
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            // Find nearest CanvasView in the logical tree
           _canvas = this.FindAncestorOfType<CanvasView>();

        }

        private void BrushSetting_ValueChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
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

        private void ColorWheel_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (_canvas == null) return;

            if (e.Property.Name == nameof(Controls.ColorWheel.ActiveColor) && sender is Controls.ColorWheel colorWheel)
            {
                Color c = colorWheel.ActiveColor;
                _canvas.ActiveColor = (uint)((c.A << 24) | (c.R << 16) | (c.G << 8) | c.B);
            }
        }
    }
}
