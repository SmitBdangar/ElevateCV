using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Platform.Storage;
using Luminos.Views;
using Luminos.Core;


namespace Luminos
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Helper to find the CanvasView instance
        private CanvasView? FindCanvasView()
        {
            return this.GetVisualDescendants()
                       .OfType<CanvasView>()
                       .FirstOrDefault();
        }

        // --- FILE MENU COMMANDS ---
        public void OnNewClicked(object? sender, RoutedEventArgs e)
        {
            // TODO: Implement new document
        }

        public void OnOpenClicked(object? sender, RoutedEventArgs e)
        {
            // TODO: Implement open
        }

        public void OnSaveClicked(object? sender, RoutedEventArgs e)
        {
            // TODO: Implement save
        }

        public async void OnExportPngClicked(object? sender, RoutedEventArgs e)
        {
            var canvas = FindCanvasView();
            if (canvas == null)
                return;

            var storageProvider = TopLevel.GetTopLevel(this)?.StorageProvider;
            if (storageProvider == null)
                return;

            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export PNG",
                SuggestedFileName = "Luminos_Image",
                FileTypeChoices = new[] { FilePickerFileTypes.ImagePng }
            });

            if (file == null || canvas.CanvasBitmap == null)
                return;

            await FileHandler.ExportPng(canvas.CanvasBitmap, file);
        }

        // --- EDIT MENU COMMANDS ---
        public void OnUndoClicked(object? sender, RoutedEventArgs e)
        {
            FindCanvasView()?.Undo();
        }

        public void OnRedoClicked(object? sender, RoutedEventArgs e)
        {
            FindCanvasView()?.Redo();
        }
    }
}
