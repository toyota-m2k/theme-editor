using io.github.toyota32k.toolkit.net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;

namespace ThemeEditor;

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

public static class AndroidColorUtility {
    public static List<NamedColor> Primary(this List<NamedColor> list) {
        return list.Where(it => it.Name.ToLower().Contains("primary")).ToList();
    }
    public static List<NamedColor> Secondary(this List<NamedColor> list) {
        return list.Where(it => it.Name.ToLower().Contains("secondary")).ToList();
    }
    public static List<NamedColor> Tertiary(this List<NamedColor> list) {
        return list.Where(it => it.Name.ToLower().Contains("tertiary")).ToList();
    }
    public static List<NamedColor> Surface(this List<NamedColor> list) {
        return list.Where(it => it.Name.ToLower().Contains("surface")).ToList();
    }
    public static List<NamedColor> Text(this List<NamedColor> list) {
        return list.Where(it => it.Name.StartsWith("on")).ToList();
    }
    public static List<NamedColor> Background(this List<NamedColor> list) {
        return list.Where(it => !it.Name.StartsWith("on")).ToList();
    }
}

public class AndroidColors {
    private HashSet<string> RegisterCheck = new();
    public List<NamedColor> Normal { get; } = new();
    public List<NamedColor> Medium { get; } = new();
    public List<NamedColor> High { get; } = new();



    private void dumpSubSub(List<NamedColor> list) {
        foreach (var color in list.Background()) {
            System.Diagnostics.Debug.WriteLine($"{color.Name}: {color.Color}");
        }
        foreach (var color in list.Text()) {
            System.Diagnostics.Debug.WriteLine($"{color.Name}: {color.Color}");
        }
    }
    private void dumpSub(List<NamedColor> list) {
        System.Diagnostics.Debug.WriteLine("Primary --");
        dumpSubSub(list.Primary());
        System.Diagnostics.Debug.WriteLine("Secondary --");
        dumpSubSub(list.Secondary());
        System.Diagnostics.Debug.WriteLine("Tertiary --");
        dumpSubSub(list.Tertiary());
        System.Diagnostics.Debug.WriteLine("Surface --");
        dumpSubSub(list.Surface());
    }

    private void dump() {
        System.Diagnostics.Debug.WriteLine("Normal --");
        dumpSub(Normal);
        System.Diagnostics.Debug.WriteLine("Medium --");
        dumpSub(Medium);
        System.Diagnostics.Debug.WriteLine("High --");
        dumpSub(High);
    }


    //public static AndroidColors ParseXml(StorageFile path) {
    //    var result = new AndroidColors();
    //    var doc = new XmlDocument();
    //    doc.Load(path.Path);
    //    var root = doc.DocumentElement;
    //    if (root != null) {
    //        for (int i = 0; i < root.ChildNodes.Count; i++) {
    //            var node = root.ChildNodes[i];
    //            if (node?.Name == "color") {
    //                var name = node.Attributes?.GetNamedItem("name")?.Value;
    //                var value = node.InnerText;
    //                if (name == null || value == null) throw new ArgumentException("Invalid XML format");

    //                var fragments = name.Split("_");
    //                if(fragments.Length < 3) throw new ArgumentException("Invalid Color name");
    //                var last = fragments.LastOrDefault();
    //                var list = result.Normal;
    //                switch (last) {
    //                    case "mediumContrast": list = result.Medium; break;
    //                    case "highContrast": list = result.High; break;
    //                    default:break;
    //                }
    //                list.Add(new NamedColor(fragments[2], value));
    //            }
    //        }
    //    }
    //    result.dump();
    //    return result;
    //}

    private AndroidColors AppendCore(XmlDocument doc) {
        var root = doc.DocumentElement;
        if (root != null) {
            for (int i = 0; i < root.ChildNodes.Count; i++) {
                var node = root.ChildNodes[i];
                if (node?.Name == "color") {
                    var name = node.Attributes?.GetNamedItem("name")?.Value;
                    var value = node.InnerText;
                    if (name == null || value == null) throw new ArgumentException("Invalid XML format");
                    var fragments = name.Split("_");
                    if (fragments.Length < 3) throw new ArgumentException("Invalid Color name");
                    var last = fragments.LastOrDefault() ?? "";
                    var list = Normal;
                    switch (last) {
                        case "mediumContrast": list = Medium; break;
                        case "highContrast": list = High; break;
                        default: break;
                    }
                    var key = fragments[2]+last;
                    if (!RegisterCheck.Contains(key)) {     // 重複登録チェック
                        RegisterCheck.Add(key);
                        list.Add(new NamedColor(fragments[2], value));
                    }
                }
            }
        }
        dump();
        return this;
    }

    public AndroidColors Append(StorageFile path) {
        var doc = new XmlDocument();
        doc.Load(path.Path);
        return AppendCore(doc);
    }
    public AndroidColors Append(Stream stream) {
        var doc = new XmlDocument();
        doc.Load(stream);
        return AppendCore(doc);
    }

    //public String ToXml(String baseName) {
    //    var sb = new StringBuilder();
    //    sb.AppendLine("<resources>");
    //    foreach (var color in Normal) {
    //        sb.AppendLine($"    <color name=\"{baseName}_theme_{color.Name}\">{ToRgbHex(color.Color)}</color>");
    //    }
    //    foreach (var color in Medium) {
    //        sb.AppendLine($"    <color name=\"{baseName}_theme_{color.Name}_mediumContrast\">{ToRgbHex(color.Color)}</color>");
    //    }
    //    foreach (var color in High) {
    //        sb.AppendLine($"    <color name=\"{baseName}_theme_{color.Name}_highContrast\">{ToRgbHex(color.Color)}</color>");
    //    }
    //    sb.AppendLine("</resources>");
    //    return sb.ToString();
    //}
}
