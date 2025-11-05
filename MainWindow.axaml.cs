using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Platform.Storage;
using Pixellum.Views;
using Pixellum.Core;

namespace Pixellum
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Optional: Locate ToolsPanel and set initial color when needed
            var toolsPanel = this.GetVisualDescendants()
                                 .OfType<ToolsPanel>()
                                 .FirstOrDefault();
            // Example (uncomment if your ToolsPanel exposes a SetColor method):
            // toolsPanel?.SetColor(0xFFFF0000);
        }

        // Locate the CanvasView instance in the visual tree
        private CanvasView? FindCanvasView() =>
            this.GetVisualDescendants()
                .OfType<CanvasView>()
                .FirstOrDefault();

        // --- FILE MENU COMMANDS ---
        public void OnNewClicked(object? sender, RoutedEventArgs e)
        {
            // TODO: Implement new document creation
        }

        public void OnOpenClicked(object? sender, RoutedEventArgs e)
        {
            // TODO: Implement open dialog + load
        }

        public void OnSaveClicked(object? sender, RoutedEventArgs e)
        {
            // TODO: Implement save project
        }

        public async void OnExportPngClicked(object? sender, RoutedEventArgs e)
        {
            var canvas = FindCanvasView();
            if (canvas?.CanvasBitmap == null)
                return;

            var storageProvider = TopLevel.GetTopLevel(this)?.StorageProvider;
            if (storageProvider == null)
                return;

            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export PNG",
                SuggestedFileName = "Pixellum_Image",
                FileTypeChoices = new[] { FilePickerFileTypes.ImagePng }
            });

            if (file != null)
            {
                await FileHandler.ExportPng(canvas.CanvasBitmap, file);
                UpdateStatus("✅ Exported to " + file.Name); // ✅
            }

        }

        // --- EDIT MENU COMMANDS ---
        public void OnUndoClicked(object? sender, RoutedEventArgs e) =>
            FindCanvasView()?.Undo();

        public void OnRedoClicked(object? sender, RoutedEventArgs e) =>
            FindCanvasView()?.Redo();

        private void UpdateStatus(string message)
        {
            var statusText = this.FindControl<TextBlock>("StatusText");
            if (statusText != null)
                statusText.Text = message;
        }
    }
}
