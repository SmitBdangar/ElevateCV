using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia;
using System;
using Avalonia.Markup.Xaml;

namespace Luminos.Controls
{
    // Custom HSV wheel control 
    public partial class ColorWheel : UserControl
    {
        // Define an ActiveColor property that other parts of the application can bind to
        public static readonly DirectProperty<ColorWheel, Color> ActiveColorProperty =
            AvaloniaProperty.RegisterDirect<ColorWheel, Color>(
                nameof(ActiveColor),
                o => o.ActiveColor,
                (o, v) => o.ActiveColor = v);

        private Color _activeColor = Colors.White;
        public Color ActiveColor
        {
            get => _activeColor;
            private set => SetAndRaise(ActiveColorProperty, ref _activeColor, value);
        }
        
        private bool _isSelecting = false;

        public ColorWheel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void ColorWheelSurface_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(this);
            if (point.Properties.IsLeftButtonPressed)
            {
                _isSelecting = true;
                UpdateColorFromPoint(point.Position);
                e.Handled = true;
            }
        }

        private void ColorWheelSurface_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (_isSelecting)
            {
                UpdateColorFromPoint(e.GetCurrentPoint(this).Position);
                e.Handled = true;
            }
        }

        private void ColorWheelSurface_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _isSelecting = false;
        }
        
        // This is where the complex math happens: mapping a coordinate to an HSV color.
        private void UpdateColorFromPoint(Point p)
        {
            double width = Bounds.Width;
            double height = Bounds.Height;
            
            // Simplified calculation: we'll just set a dummy color for MVP
            if (p.X > width / 2)
            {
                ActiveColor = Colors.Red;
            }
            else
            {
                ActiveColor = Colors.Blue;
            }
            
            // FUTURE: Implement the actual HSV conversion logic:
            // 1. Calculate distance and angle from center (for Saturation and Hue).
            // 2. Convert H, S, V to RGB/Avalonia Color.
            // 3. Force a redraw of the color wheel surface.
        }

        // FUTURE: Override Render method to draw the circular color gradient and selection marker.
    }
}
