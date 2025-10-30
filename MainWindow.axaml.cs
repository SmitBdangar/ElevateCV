using Avalonia.Controls;
using Avalonia.Media;
using Luminos.Rendering;

namespace Luminos
{
    public partial class MainWindow : Window
    {
        // Use the global shared instance
        public BrushEngine BrushEngine => BrushEngine.Instance;

        public MainWindow()
        {
            InitializeComponent();

            // Color selection buttons
            RedColor.PointerPressed += (_, __) => BrushEngine.BrushColor = Colors.Red;
            GreenColor.PointerPressed += (_, __) => BrushEngine.BrushColor = Colors.Green;
            BlueColor.PointerPressed += (_, __) => BrushEngine.BrushColor = Colors.Blue;
            YellowColor.PointerPressed += (_, __) => BrushEngine.BrushColor = Colors.Yellow;
            WhiteColor.PointerPressed += (_, __) => BrushEngine.BrushColor = Colors.White;
            BlackColor.PointerPressed += (_, __) => BrushEngine.BrushColor = Colors.Black;

            // Brush Size Slider
            BrushSizeSlider.PropertyChanged += (_, e) =>
            {
                if (e.Property.Name == nameof(BrushSizeSlider.Value))
                    BrushEngine.BrushSize = (int)BrushSizeSlider.Value;
            };
        }
    }
}

