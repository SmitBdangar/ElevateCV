using Luminos.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luminos.Rendering
{
    /// <summary>
    /// Handles combining all layers in a Document into the final composite pixel buffer.
    /// Phase 1: Simple copy of the active layer's pixels.
    /// Phase 2: Will implement blend modes and opacity for each layer.
    /// </summary>
    public static class LayerCompositor
    {
        /// <summary>
        /// Composites the provided layers into the Document's main pixel buffer.
        /// </summary>
        /// <param name="document">The target document whose buffer will be updated.</param>
        /// <param name="layers">The collection of layers to composite (assumed top-down order).</param>
        public static void Composite(Document document, IEnumerable<Layer> layers)
        {
            uint[] documentPixels = document.GetPixelsRaw();
            
            // Find the top-most visible layer to render for the MVP
            var topLayer = layers.FirstOrDefault(l => l.Visible); 
            
            if (topLayer == null)
            {
                // If no layers are visible, clear the document buffer to transparent.
                Array.Clear(documentPixels, 0, documentPixels.Length);
                return;
            }

            // --- Phase 1 MVP Logic: Direct Copy ---
            // In Phase 1, we just copy the pixels from the active layer to the document's buffer.
            uint[] layerPixels = topLayer.GetPixels();

            // Use fast Array.Copy
            Array.Copy(layerPixels, documentPixels, layerPixels.Length);
            
            // NOTE: Phase 2 will involve iterating through all layers and pixel-by-pixel blending 
            // (using BlendModes.cs) based on their blend mode and opacity.
        }
    }
}