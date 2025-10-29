using SkiaSharp;
using Luminos.Core;

namespace Luminos.Rendering;

public static class BrushEngine
{
    private static SKPoint? _lastPoint = null;

    public static void DrawStroke(Document document, float x, float y, float size, SKColor color)
    {
        var layer = document.Layers[0];
        using var canvas = new SKCanvas(layer.Bitmap);

        using var paint = new SKPaint
        {
            Color = color,
            IsAntialias = true,
            StrokeWidth = size,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        var current = new SKPoint(x, y);

        if (_lastPoint == null)
        {
            canvas.DrawPoint(current, paint);
        }
        else
        {
            canvas.DrawLine(_lastPoint.Value, current, paint);
        }

        _lastPoint = current;
    }

    public static void EndStroke()
    {
        _lastPoint = null;
    }
}
