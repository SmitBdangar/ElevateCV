using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using System;

namespace Pixellum.Controls
{
    public partial class HexColorGrid : UserControl
    {
        public event Action<Color>? ColorSelected;

        public static readonly StyledProperty<Color[][]> PaletteProperty =
            AvaloniaProperty.Register<HexColorGrid, Color[][]>(nameof(Palette));

        public Color[][] Palette
        {
            get => GetValue(PaletteProperty);
            set => SetValue(PaletteProperty, value);
        }

        public HexColorGrid()
        {
            InitializeComponent();
            AttachedToVisualTree += (_, __) => Build();
        }

        private void Build()
        {
            var grid = this.FindControl<UniformGrid>("GridRoot");
            if (grid == null || Palette == null) return;

            grid.Children.Clear();

            foreach (var row in Palette)
            {
                foreach (var c in row)
                {
                    var cell = new Border
                    {
                        Width = 28,
                        Height = 28,
                        CornerRadius = new CornerRadius(6),
                        Background = new SolidColorBrush(c),
                        Margin = new Thickness(3),
                        BorderBrush = Brushes.Transparent,   // keep layout stable
                        BorderThickness = new Thickness(2),  // thin outline
                        Cursor = new Cursor(StandardCursorType.Hand)
                    };


                    cell.PointerPressed += (_, __) => ColorSelected?.Invoke(c);
                    cell.PointerEntered += (_, __) =>
                        cell.BorderBrush = Brushes.White;

                    cell.PointerExited += (_, __) =>
                        cell.BorderBrush = Brushes.Transparent;

                    grid.Children.Add(cell);
                }
            }
        }
    }
}
