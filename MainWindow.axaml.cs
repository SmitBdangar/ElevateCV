using Avalonia.Controls;
using Avalonia.Media;
using Luminos.Rendering;
using Luminos.Controls;

namespace Luminos
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 🎨 Color Wheel → Update Brush Color
            ColorWheel.ColorChanged += (Color color) =>
            {
                BrushEngine.Instance.IsEraser = false;
                BrushEngine.Instance.BrushColor = color;
            };

            // ✏ Brush Size Slider → Update Brush Size
            BrushSizeSlider.PropertyChanged += (_, e) =>
            {
                if (e.Property.Name == "Value")
                    BrushEngine.Instance.BrushSize = (int)BrushSizeSlider.Value;
            };

            // 🧽 Eraser Button → Switch to Eraser
            EraserButton.Click += (_, __) =>
            {
                BrushEngine.Instance.IsEraser = true;
            };
        }
    }
}
