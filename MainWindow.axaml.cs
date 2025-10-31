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

            ColorWheel.ColorChanged += color =>
            {
                BrushEngine.Instance.IsEraser = false;
                BrushEngine.Instance.BrushColor = color;
            };

            BrushSizeSlider.PropertyChanged += (_, e) =>
            {
                if (e.Property.Name == "Value")
                    BrushEngine.Instance.BrushSize = (int)BrushSizeSlider.Value;
            };

            EraserButton.Click += (_, __) =>
            {
                BrushEngine.Instance.IsEraser = true;
            };
        }
    }
}
