using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using Pixellum.Core;

namespace Pixellum.Views
{
    public partial class LayersPanel : UserControl
    {
        private CanvasView? _canvas;
        private int _layerCounter = 1;

        public LayersPanel()
        {
            InitializeComponent();

            // Find canvas after UI loads
            this.Loaded += (_, __) =>
            {
                var window = this.GetVisualRoot() as Window;
                if (window != null)
                {
                    _canvas = window.GetVisualDescendants()
                                    .OfType<CanvasView>()
                                    .FirstOrDefault();

                    // Show existing layers
                    if (_canvas != null)
                    {
                        RefreshLayersList();
                    }
                }
            };
        }

        private void OnAddLayerClicked(object? sender, RoutedEventArgs e)
        {
            if (_canvas == null) return;

            // Add new layer to canvas
            var newLayerName = $"Layer {_layerCounter++}";
            _canvas.AddLayer(newLayerName);

            // Refresh the UI
            RefreshLayersList();
        }

        private void RefreshLayersList()
        {
            if (_canvas == null) return;

            var stack = this.FindControl<StackPanel>("LayersStack");
            if (stack == null) return;

            stack.Children.Clear();

            var layers = _canvas.GetLayers();
            int activeIndex = _canvas.GetActiveLayerIndex();

            // Add layer items in reverse order (top layer first)
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                var layer = layers[i];
                bool isActive = (i == activeIndex);

                var layerItem = CreateLayerItem(layer.Name, i, isActive);
                stack.Children.Add(layerItem);
            }
        }

        private Border CreateLayerItem(string layerName, int layerIndex, bool isActive)
        {
            var border = new Border
            {
                Background = isActive ? new SolidColorBrush(Color.Parse("#666")) : new SolidColorBrush(Color.Parse("#444")),
                BorderBrush = isActive ? Brushes.White : Brushes.Transparent,
                BorderThickness = new Avalonia.Thickness(2),
                CornerRadius = new Avalonia.CornerRadius(5),
                Padding = new Avalonia.Thickness(10),
                Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
            };

            // Create horizontal layout with layer name and delete button
            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("*,Auto")
            };

            // Container that can switch between TextBlock and TextBox
            var contentPanel = new Panel();
            Grid.SetColumn(contentPanel, 0);

            var textBlock = new TextBlock
            {
                Text = layerName,
                Foreground = Brushes.White,
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center
            };

            var textBox = new TextBox
            {
                Text = layerName,
                Foreground = Brushes.White,
                FontSize = 14,
                Background = new SolidColorBrush(Color.Parse("#555")),
                BorderBrush = Brushes.White,
                BorderThickness = new Avalonia.Thickness(1),
                IsVisible = false,
                VerticalAlignment = VerticalAlignment.Center
            };

            contentPanel.Children.Add(textBlock);
            contentPanel.Children.Add(textBox);

            // Delete button
            var deleteButton = new Button
            {
                Content = "×",
                FontSize = 20,
                FontWeight = FontWeight.Bold,
                Width = 30,
                Height = 30,
                Background = new SolidColorBrush(Color.Parse("#d32f2f")),
                Foreground = Brushes.White,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Avalonia.Thickness(0),
                Margin = new Avalonia.Thickness(5, 0, 0, 0),
                CornerRadius = new Avalonia.CornerRadius(3),
                Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
            };
            Grid.SetColumn(deleteButton, 1);

            // Delete button click handler
            deleteButton.Click += (_, __) =>
            {
                if (_canvas != null && _canvas.GetLayers().Count > 1) // Prevent deleting last layer
                {
                    _canvas.DeleteLayer(layerIndex);
                    RefreshLayersList();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Cannot delete the last layer");
                }
            };

            // Single click to select layer
            textBlock.PointerPressed += (_, e) =>
            {
                _canvas?.SetActiveLayer(layerIndex);
                RefreshLayersList();
            };

            // Double click to rename
            textBlock.DoubleTapped += (_, __) =>
            {
                textBlock.IsVisible = false;
                textBox.IsVisible = true;
                textBox.Focus();
                textBox.SelectAll();
            };

            // Save rename on Enter or losing focus
            textBox.KeyDown += (_, e) =>
            {
                if (e.Key == Avalonia.Input.Key.Enter)
                {
                    FinishRename(layerIndex, textBox, textBlock);
                }
                else if (e.Key == Avalonia.Input.Key.Escape)
                {
                    textBox.Text = layerName; // Reset to original
                    textBlock.IsVisible = true;
                    textBox.IsVisible = false;
                }
            };

            textBox.LostFocus += (_, __) =>
            {
                FinishRename(layerIndex, textBox, textBlock);
            };

            grid.Children.Add(contentPanel);
            grid.Children.Add(deleteButton);

            border.Child = grid;

            return border;
        }

        private void FinishRename(int layerIndex, TextBox textBox, TextBlock textBlock)
        {
            string newName = textBox.Text?.Trim() ?? "";
            
            if (!string.IsNullOrWhiteSpace(newName))
            {
                _canvas?.RenameLayer(layerIndex, newName);
                textBlock.Text = newName;
            }
            
            textBlock.IsVisible = true;
            textBox.IsVisible = false;
        }
    }
}