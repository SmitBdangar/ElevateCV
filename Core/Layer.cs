using System;
namespace Luminos.Core
{
    // Placeholder enum for planned blend modes. [cite: 73, 106]
    public enum BlendMode
    {
        Normal, 
        Multiply,
        Screen,
        Overlay
    }

    /// <summary>
    /// Represents a single layer within the Document. [cite: 29]
    /// </summary>
    public class Layer
    {
        public string Name { get; set; }
        public bool Visible { get; set; } = true;
        
        // Opacity should be a value between 0.0 and 1.0. [cite: 73]
        private float _opacity = 1.0f;
        public float Opacity
        {
            get => _opacity;
            set => _opacity = Math.Clamp(value, 0.0f, 1.0f);
        }
        
        public BlendMode Mode { get; set; } = BlendMode.Normal;

        // The per-layer pixel buffer. [cite: 73]
        public int Width { get; }
        public int Height { get; }
        private readonly uint[] _pixels;

        public Layer(int width, int height, string name = "Layer")
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException("Width and Height must be positive.");
            }
            Width = width;
            Height = height;
            Name = name;
            
            _pixels = new uint[Width * Height];
        }

        public uint[] GetPixels() => _pixels;

        public void Clear()
        {
            Array.Clear(_pixels, 0, _pixels.Length);
        }
    }
}
