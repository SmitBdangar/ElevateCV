using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using System.IO;
using System;

using System.Threading.Tasks;

namespace Luminos.Core
{
    public static class FileHandler
    {
        public static async Task ExportPng(WriteableBitmap bitmap, IStorageFile storageFile)
        {
            try
            {
                await using var stream = await storageFile.OpenWriteAsync();
                stream.SetLength(0);
                bitmap.Save(stream);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Export failed: {ex.Message}");
                // TODO: Show error dialog to user
            }
        }
    }
}
