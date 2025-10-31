using Luminos.Core; 
using System;

namespace Luminos.Rendering
{
    public class BrushEngine
    {
        /// <summary>
        /// Applies a circular brush stroke at the specified coordinates on a given layer.
        /// </summary>
        public void ApplyBrush(Layer layer, int centerX, int centerY, uint brushColor, float radius)
        {
            float srcA = ((brushColor >> 24) & 0xFF) / 255.0f;
            uint[] layerPixels = layer.GetPixels();

            int minX = Math.Max(0, (int)(centerX - radius));
            int maxX = Math.Min(layer.Width, (int)(centerX + radius));
            int minY = Math.Max(0, (int)(centerY - radius));
            int maxY = Math.Min(layer.Height, (int)(centerY + radius));
            
            float radiusSq = radius * radius;

            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    // Check if the pixel is within the circular radius [cite: 76]
                    float dX = x - centerX;
                    float dY = y - centerY;
                    if (dX * dX + dY * dY <= radiusSq)
                    {
                        int index = y * layer.Width + x;
                        uint dstPixel = layerPixels[index];

                        // Blend the brush color (source) over the existing layer pixel (destination)
                        uint resultPixel = AlphaBlendPremultiplied(brushColor, dstPixel, srcA);
                        
                        layerPixels[index] = resultPixel;
                    }
                }
            }
        }
        
        /// <summary>
        /// Performs the 'Source Over Destination' alpha blending using the premultiplied alpha formula[cite: 78].
        /// </summary>
        /// <remarks>Formula: outRGB = srcRGB + dstRGB* (1 - srcA); outA = srcA + dstA* (1 - srcA) [cite: 79, 80]</remarks>
        private uint AlphaBlendPremultiplied(uint src, uint dst, float srcA)
        {
            // Extract and normalize components (0.0 to 1.0)
            float dstA = ((dst >> 24) & 0xFF) / 255.0f;
            float dstR = ((dst >> 16) & 0xFF) / 255.0f;
            float dstG = ((dst >> 8) & 0xFF) / 255.0f;
            float dstB = (dst & 0xFF) / 255.0f;

            float srcR = ((src >> 16) & 0xFF) / 255.0f;
            float srcG = ((src >> 8) & 0xFF) / 255.0f;
            float srcB = (src & 0xFF) / 255.0f;
            
            float invSrcA = 1.0f - srcA;

            // Apply the Alpha Blending formula
            float outR = srcR * srcA + dstR * invSrcA;
            float outG = srcG * srcA + dstG * invSrcA;
            float outB = srcB * srcA + dstB * invSrcA;
            float outA = srcA + dstA * invSrcA;

            // Convert back to 0-255 integer range and recombine
            uint A = (uint)Math.Clamp(outA * 255.0f, 0, 255);
            uint R = (uint)Math.Clamp(outR * 255.0f, 0, 255);
            uint G = (uint)Math.Clamp(outG * 255.0f, 0, 255);
            uint B = (uint)Math.Clamp(outB * 255.0f, 0, 255);

            return (A << 24) | (R << 16) | (G << 8) | B;
        }

        // Implements the Eraser tool functionality [cite: 103]
        public void ApplyEraser(Layer layer, int centerX, int centerY, float radius)
        {
            // Erasing is handled by painting with a transparent black source color (0x00000000).
            ApplyBrush(layer, centerX, centerY, 0x00000000, radius); 
        }
    }
}
