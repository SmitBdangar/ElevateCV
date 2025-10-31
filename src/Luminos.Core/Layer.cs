using System;

namespace Luminos.Core
{
    public enum BlendMode
    {
        Normal,
        Multiply,
        Screen,
        Overlay
    }

    public class Layer
    {
        public string Name { get; set; }
        public bool Visible { get; set; } = true;

        private float _opacity = 1.0f;
        public float Opacity
        {
            get => _opacity;
            set => _opacity = Math.Clamp(value, 0.0f, 1.0f);
        }

        public BlendMode Mode { get; set; } = BlendMode.Normal;

        public int Width { get; }
        public int Height { get; }
        private readonly uint[] _pixels;

        public Layer(int width, int height, string name = "Layer")
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Width and Height must be positive.");

            Width = width;
            Height = height;
            Name = name;
            _pixels = new uint[Width * Height];
        }

        public uint[] GetPixels() => _pixels;

        public void Clear() => Array.Clear(_pixels, 0, _pixels.Length);
    }
}
