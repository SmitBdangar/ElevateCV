using Xunit;
using Luminos.Rendering;
using Luminos.Core;

namespace Luminos.Tests
{
    public class RenderingTests
    {
        // A helper function to create a BrushEngine instance for testing the private methods
        // NOTE: While a full solution would require reflection or moving the method, 
        // we'll assume the blending math is correct based on the BrushEngine implementation.
        // We will test the public ApplyBrush method instead.

        [Fact]
        public void BrushEngine_ApplyBrush_PaintsOpaqueColor()
        {
            // Arrange: 1x1 layer
            var layer = new Layer(1, 1);
            var brush = new BrushEngine();
            uint opaqueRed = 0xFFFF0000;
            
            // Act
            // Paint the center pixel (0, 0) with radius 1
            brush.ApplyBrush(layer, 0, 0, opaqueRed, 1.0f);

            // Assert: The pixel should now be opaque red
            Assert.Equal(opaqueRed, layer.GetPixels()[0]);
        }

        [Fact]
        public void BrushEngine_ApplyBrush_PaintsOverTransparent()
        {
            // Arrange: 1x1 layer, transparent black
            var layer = new Layer(1, 1);
            var brush = new BrushEngine();
            uint fiftyPercentBlue = 0x800000FF; // 50% opacity blue
            
            // Act
            brush.ApplyBrush(layer, 0, 0, fiftyPercentBlue, 1.0f);

            // Assert: Result should be 50% opacity blue (src over transparent dest)
            Assert.Equal(fiftyPercentBlue, layer.GetPixels()[0]);
        }
        
        [Fact]
        public void BrushEngine_ApplyEraser_ClearsPixel()
        {
            // Arrange: 1x1 layer, full opaque color
            var layer = new Layer(1, 1);
            var brush = new BrushEngine();
            layer.GetPixels()[0] = 0xFFCCCCCC; // Opaque gray
            
            // Act
            brush.ApplyEraser(layer, 0, 0, 1.0f);

            // Assert: The pixel should be transparent black (0x00000000)
            Assert.Equal(0x00000000, layer.GetPixels()[0]);
        }
    }
}