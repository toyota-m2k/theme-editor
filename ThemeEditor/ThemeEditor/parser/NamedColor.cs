using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThemeEditor.parser;

public class NamedColor {
    public static Color ColorFromHexString(string hexColor) {
        // Remove "#" if present
        hexColor = hexColor.TrimStart('#');
        // Parse depending on the length (6 digits = RGB, 8 digits = ARGB)
        if (hexColor.Length == 6) {
            int r = Convert.ToInt32(hexColor[..2], 16);
            int g = Convert.ToInt32(hexColor.Substring(2, 2), 16);
            int b = Convert.ToInt32(hexColor.Substring(4, 2), 16);
            return Color.FromArgb(r, g, b);
        }
        else if (hexColor.Length == 8) {
            int a = Convert.ToInt32(hexColor[..2], 16);
            int r = Convert.ToInt32(hexColor.Substring(2, 2), 16);
            int g = Convert.ToInt32(hexColor.Substring(4, 2), 16);
            int b = Convert.ToInt32(hexColor.Substring(6, 2), 16);
            return Color.FromArgb(a, r, g, b);
        }
        throw new ArgumentException("Invalid color format", nameof(hexColor));
    }

    public static string ColorToHexString(Color color) {
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }


    public string Name { get; }
    public Color Color { get; }
    public Windows.UI.Color AsUiColor => Windows.UI.Color.FromArgb(Color.A, Color.R, Color.G, Color.B);
    public Microsoft.UI.Xaml.Media.Brush Brush { get; }
    public string ColorCode => ColorToHexString(Color);
    public NamedColor(string name, Color color) {
        Name = name;
        Color = color;
        Brush = new Microsoft.UI.Xaml.Media.SolidColorBrush(AsUiColor);
    }
    public NamedColor(string name, string value) {
        Name = name;
        Color = ColorFromHexString(value);
        Brush = new Microsoft.UI.Xaml.Media.SolidColorBrush(AsUiColor);
    }

    public NamedColor NewColor(Color color) {
        return new NamedColor(this.Name, color);
    }

    public static NamedColor Empty = new NamedColor("", Color.Empty);
    public bool IsEmpty => Color.IsEmpty;
}
