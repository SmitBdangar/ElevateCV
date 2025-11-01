using System;

namespace Luminos.Core
{
    /// <summary>
    /// Utility methods for manipulating 1D pixel arrays, primarily for Delta Record extraction.
    /// </summary>
    public static class PixelUtils
    {
        /// <summary>
        /// Extracts a rectangular region of pixels from a flat, 1D source array into a destination array.
        /// </summary>
        public static void GetRegionPixels(uint[] source, int sourceWidth, IntRect rect, uint[] destination)
        {
            if (rect.IsEmpty) return;
            
            // Defensive check: Ensure destination is sized correctly
            int expectedSize = rect.Width * rect.Height;
            if (destination.Length < expectedSize)
            {
                throw new ArgumentException("Destination array must be large enough to hold the region.");
            }
            
            int destinationIndex = 0;
            for (int y = rect.Y; y < rect.Y + rect.Height; y++)
            {
                // Source index is the start of the current row in the layer's flat buffer
                int sourceStart = y * sourceWidth + rect.X;
                
                // Copy one row's worth of pixels from source to destination
                Array.Copy(source, sourceStart, destination, destinationIndex, rect.Width);
                
                destinationIndex += rect.Width;
            }
        }
    }
}