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

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            base.OnClosing(e);
        }

        private CanvasView? FindCanvasView()
        {
            return this.GetVisualDescendants()
                       .OfType<CanvasView>()
                       .FirstOrDefault();
        }

        public async void OnExportPngClicked(object sender, RoutedEventArgs e)
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

            // ✅ Defensive null-check
            if (file == null || canvas.CanvasBitmap == null)
                return;

            await FileHandler.ExportPng(canvas.CanvasBitmap, file);
        }

    }
}
