using Avalonia.Controls;
using Avalonia.Media;
using Luminos.Rendering;

namespace Luminos
{
    public partial class MainWindow : Window
    {
        public BrushEngine BrushEngine => BrushEngine.Instance;

        public MainWindow()
        {
            InitializeComponent();

            // ✅ Use SetBrushColor so eraser mode turns off when selecting a color
            RedColor.PointerPressed += (_, __) => SetBrushColor(Colors.Red);
            GreenColor.PointerPressed += (_, __) => SetBrushColor(Colors.Green);
            BlueColor.PointerPressed += (_, __) => SetBrushColor(Colors.Blue);
            YellowColor.PointerPressed += (_, __) => SetBrushColor(Colors.Yellow);
            WhiteColor.PointerPressed += (_, __) => SetBrushColor(Colors.White);
            BlackColor.PointerPressed += (_, __) => SetBrushColor(Colors.Black);

            // ✅ Eraser Mode
            EraserButton.PointerPressed += (_, __) => BrushEngine.IsEraser = true;

            // ✅ Brush Size Slider
            BrushSizeSlider.PropertyChanged += (_, e) =>
            {
                if (e.Property.Name == "Value")
                    BrushEngine.BrushSize = (int)BrushSizeSlider.Value;
            };

            // ✅ Brush Opacity Slider
            BrushOpacitySlider.PropertyChanged += (_, e) =>
            {
                if (e.Property.Name == "Value")
                    BrushEngine.BrushOpacity = BrushOpacitySlider.Value;
            };
        }

        // ✅ Centralized color setter — automatically disables eraser mode
        private void SetBrushColor(Color color)
        {
            BrushEngine.IsEraser = false;
            BrushEngine.BrushColor = color;
        }
    }
}
