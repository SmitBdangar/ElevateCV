using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using System.IO;
using System.Threading.Tasks;

namespace Luminos.Core
{
    public static class FileHandler
    {
        /// <summary>
        /// Saves a WriteableBitmap as a PNG to the selected storage file.
        /// Automatically truncates the file to prevent PNG corruption.
        /// </summary>
        public static async Task ExportPng(WriteableBitmap bitmap, IStorageFile storageFile)
        {
            // Safety checks
            if (bitmap == null || storageFile == null)
                return;

            // Open the file for writing
            await using var stream = await storageFile.OpenWriteAsync();

            // IMPORTANT:
            // Avalonia **appends** if we don't reset the file length.
            // This prevents corrupted PNGs.
            stream.SetLength(0);

            // Avalonia automatically encodes WriteableBitmap as PNG
            bitmap.Save(stream);
        }
    }
}
