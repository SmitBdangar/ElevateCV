namespace Luminos.Core;
using System.Collections.Generic;
using System;

public class Document
{
    public int Width { get; }
    public int Height { get; }
    public List<Layer> Layers { get; } = new();

    public Document(int width, int height)
    {
        Width = width;
        Height = height;

        // Start with one blank layer
        Layers.Add(Layer.CreateBlank(width, height));
    }
}
