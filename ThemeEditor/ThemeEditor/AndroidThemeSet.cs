using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Windows.Storage;
using static System.Windows.Forms.DataFormats;

namespace ThemeEditor;

public enum ColorContrast {
    Normal,
    Medium,
    High,
}

public enum ExchangeColorTarget {
    Primary_Secondary,
    Primary_Tertiary,
    Secondary_Tertiary,
}

public enum CommonThemeType {
    Primary,
    Secondary,
    Tertiary,
}

public class AndroidContrastColorTheme {
    private AndroidColors Colors;
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
}

public enum DayNightMode {
    Light,
    Dark,
}

public class AndroidColorThemeSet {
    public AndroidColorThemeSet(AndroidContrastColorTheme light, AndroidContrastColorTheme dark) {
        Light = light;
        Dark = dark;
    }

    public AndroidContrastColorTheme Light { get; }
    public AndroidContrastColorTheme Dark { get; }

    public AndroidColorTheme ThemeOf(DayNightMode mode, ColorContrast contrast) {
        return mode switch {
            DayNightMode.Light => Light.ThemeOf(contrast),
            DayNightMode.Dark => Dark.ThemeOf(contrast),
            _ => throw new ArgumentException("Invalid mode", nameof(mode)),
        };
    }

    public void ExchangeColors(ExchangeColorTarget target) {
        Light.ExchangeColors(target);
        Dark.ExchangeColors(target);
    }

    public class ThemeBuilder {
        AndroidColors Light = new();
        AndroidColors Dark = new();

        public ThemeBuilder AppendLight(StorageFile path) {
            try {
                Light.Append(path);
            }
            catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return this;
        }
        public ThemeBuilder AppendLight(Stream stream) {
            try {
                Light.Append(stream);
            }
            catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return this;
        }
        public ThemeBuilder AppendDark(StorageFile path) {
            try {
                Dark.Append(path);
            }
            catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return this;
        }
        public ThemeBuilder AppendDark(Stream stream) {
            try {
                Dark.Append(stream);
            }
            catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return this;
        }


        public ThemeBuilder AppendAuto(StorageFile path) {
            if (path.FileType == ".xml") {
                if (path.Path.Contains("night")) {
                    return AppendDark(path);
                }
                else {
                    AppendLight(path);
                }
            }
            else if (path.FileType == ".zip") {
                try {
                    using var stream = path.OpenStreamForReadAsync().Result;
                    var archive = new ZipArchive(stream);
                    foreach (var entry in archive.Entries) {
                        var innerStream = entry.Open();
                        if (entry.FullName.Contains("night")) {
                            AppendDark(innerStream);
                        }
                        else {
                            AppendLight(innerStream);
                        }
                    }
                }
                catch (Exception e) {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
            return this;
        }
        public ThemeBuilder AppendDirectory(StorageFolder folder) {
            foreach (var file in folder.GetFilesAsync().AsTask().Result) {
                AppendAuto(file);
            }
            return this;
        }
        public AndroidColorThemeSet Build() {
            return new AndroidColorThemeSet(new AndroidContrastColorTheme(Light), new AndroidContrastColorTheme(Dark));
        }
    }

    public static ThemeBuilder Builder => new();
}