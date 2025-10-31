using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using System.IO;
using System.Threading.Tasks;

namespace Luminos.Core
{
    public static class FileHandler
    {
        /// <summary>
        /// Saves a WriteableBitmap as a PNG to the given storage file.
        /// </summary>
        public static async Task ExportPng(WriteableBitmap bitmap, IStorageFile storageFile)
        {
            if (bitmap == null || storageFile == null)
                return;

            // Open writable stream
            await using var stream = await storageFile.OpenWriteAsync();

            // IMPORTANT: Truncate the file before writing or Avalonia will append over old data
            stream.SetLength(0);

            // Encode as PNG
            bitmap.Save(stream); // Avalonia has built-in PNG encoding
        }
    }
}
