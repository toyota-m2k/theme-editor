using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThemeEditor.colors.auto;
using ThemeEditor.parser;

namespace ThemeEditor.colors;

/**
 * コントラスト(Normal/Medium/High)ごとのカラーセット（AndroidColorTheme）を保持するクラス
 */
public class AndroidContrastColorTheme {
    private AndroidColors Colors;   // XMLから読み込んだカラーセット原本

    public AndroidContrastColorTheme(AndroidColors colors) {
        Colors = colors;
    }

    private AndroidColorTheme? mNormal = null;
    private AndroidColorTheme? mMedium = null;
    private AndroidColorTheme? mHigh = null;

    public AndroidColorTheme NormalTheme {
        get {
            if (mNormal == null) {
                mNormal = AndroidColorTheme.Create(Colors, ColorContrast.Normal);
            }
            return mNormal!;
        }
    }
    public AndroidColorTheme MediumTheme {
        get {
            if (mMedium == null) {
                mMedium = AndroidColorTheme.Create(Colors, ColorContrast.Medium);
            }
            return mMedium!;
        }
    }
    public AndroidColorTheme HighTheme {
        get {
            if (mHigh == null) {
                mHigh = AndroidColorTheme.Create(Colors, ColorContrast.High);
            }
            return mHigh!;
        }
    }

    public AndroidColorTheme ThemeOf(ColorContrast contrast) {
        return contrast switch {
            ColorContrast.Normal => NormalTheme,
            ColorContrast.Medium => MediumTheme,
            ColorContrast.High => HighTheme,
            _ => throw new ArgumentException("Invalid contrast", nameof(contrast)),
        };
    }

    public string ToXml(string baseName) {
        var sb = new StringBuilder();
        sb.AppendLine("<resources>");
        foreach (var color in Colors.Normal) {
            var custmizedColor = NormalTheme.GetColorByName(color.Name) ?? color;
            sb.AppendLine($"    <color name=\"{baseName}_theme_{color.Name}\">{NamedColor.ColorToHexString(custmizedColor.Color)}</color>");
        }
        foreach (var color in Colors.Medium) {
            var custmizedColor = MediumTheme.GetColorByName(color.Name) ?? color;
            sb.AppendLine($"    <color name=\"{baseName}_theme_{color.Name}_mediumContrast\">{NamedColor.ColorToHexString(custmizedColor.Color)}</color>");
        }
        foreach (var color in Colors.High) {
            var custmizedColor = HighTheme.GetColorByName(color.Name) ?? color;
            sb.AppendLine($"    <color name=\"{baseName}_theme_{color.Name}_highContrast\">{NamedColor.ColorToHexString(custmizedColor.Color)}</color>");
        }
        sb.AppendLine("</resources>");
        return sb.ToString();

    }

    public void ExchangeColors(ExchangeColorTarget target) {
        NormalTheme.ExchangeColors(target);
        MediumTheme.ExchangeColors(target);
        HighTheme.ExchangeColors(target);
    }

    /// <summary>
    /// Regenerates every Medium/High role from the (possibly edited) Normal theme
    /// purely from the baked-in standard Material tone table (see
    /// <see cref="StandardTones"/>): each role's Normal color keeps its hue &amp;
    /// chroma and has its lightness (L* == HCT tone) shifted by the standard
    /// Normal->Medium / Normal->High tone delta for that role and mode. This does
    /// not read the imported Medium/High palette, so it is stable across
    /// edit -> save -> reload round-trips. Roles not in the standard table, or with
    /// an empty Normal color, are left untouched.
    /// </summary>
    public void GenerateContrastFromNormal(DayNightMode mode) {
        foreach (var origNormal in Colors.Normal) {
            if (origNormal.IsEmpty) continue;
            var role = origNormal.Name;
            var tones = StandardTones.Get(role, mode);
            if (tones == null) continue;
            var currentNormal = NormalTheme.GetColorByName(role);
            if (currentNormal == null || currentNormal.IsEmpty) continue;

            var t = tones.Value;
            var medium = ColorSpace.ShiftLightness(currentNormal.Color, t.Medium - t.Normal);
            var high = ColorSpace.ShiftLightness(currentNormal.Color, t.High - t.Normal);
            MediumTheme.SetColorByName(role, new NamedColor(role, medium));
            HighTheme.SetColorByName(role, new NamedColor(role, high));
        }
    }
}
