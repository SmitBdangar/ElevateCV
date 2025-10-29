using SkiaSharp;

namespace Luminos.Core;

public class Layer
{
    public SKBitmap Bitmap { get; private set; }

    private Layer(SKBitmap bitmap)
    {
        Bitmap = bitmap;
    }

    public static Layer CreateBlank(int width, int height)
    {
        return new Layer(new SKBitmap(width, height));
    }
}
