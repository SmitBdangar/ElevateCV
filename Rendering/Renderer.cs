using Luminos.Core; 
using Avalonia.Media.Imaging; // Required for WriteableBitmap
using Avalonia; 
using Avalonia.Platform;
using System;
using System.Runtime.CompilerServices;

namespace Luminos.Rendering
{
    /// <summary>
    /// Handles mapping the Document's raw pixel data to an Avalonia WriteableBitmap.
    /// </summary>
    public class Renderer
    {
        // Corresponds to the API: Renderer.Render(Document doc, IRenderTarget target) [cite: 117]
        /// <summary>
        /// Renders the current composite (primary layer) to the target bitmap via fast memory copy.
        /// </summary>
        public void Render(Document document, WriteableBitmap targetBitmap)
        {
            uint[] sourcePixels = document.GetPixelsRaw(); 
            
            try
            {
                // Lock the WriteableBitmap for direct memory access
                using (var lock_ = targetBitmap.Lock())
                {
                    unsafe
                    {
                        // Use unsafe Buffer.MemoryCopy for fast mem copies [cite: 83]
                        int pixelCount = document.Width * document.Height;
                        long byteCount = (long)pixelCount * sizeof(uint);
                        
                        // We must 'fix' the managed array pointer for use in the unmanaged memory copy.
                        fixed (uint* sourcePtr = sourcePixels)
                        {
                            Buffer.MemoryCopy(
                                source: sourcePtr,
                                destination: (void*)lock_.Address,
                                destinationSizeInBytes: byteCount, 
                                sourceBytesToCopy: byteCount
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // In a production app, logging would occur here.
                System.Diagnostics.Debug.WriteLine($"Rendering error: {ex.Message}");
            }
        }
    }
}
