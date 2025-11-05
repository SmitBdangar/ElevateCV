using Avalonia.Media;

namespace Pixellum.Core
{
    public static class ColorExtensions
    {
        public static Color[] ToColorArray(this string[] hex)
        {
            var arr = new Color[hex.Length];
            for (int i = 0; i < hex.Length; i++)
                arr[i] = Color.Parse(hex[i]);
            return arr;
        }
    }
}
