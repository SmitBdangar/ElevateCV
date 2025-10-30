using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Luminos.Rendering;

namespace Luminos
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            RedColor.PointerPressed += (_, __) => BrushEngine.BrushColor = Colors.Red;
            GreenColor.PointerPressed += (_, __) => BrushEngine.BrushColor = Colors.Green;
            BlueColor.PointerPressed += (_, __) => BrushEngine.BrushColor = Colors.Blue;
            YellowColor.PointerPressed += (_, __) => BrushEngine.BrushColor = Colors.Yellow;
            WhiteColor.PointerPressed += (_, __) => BrushEngine.BrushColor = Colors.White;
            BlackColor.PointerPressed += (_, __) => BrushEngine.BrushColor = Colors.Black;
        }
    }
}
