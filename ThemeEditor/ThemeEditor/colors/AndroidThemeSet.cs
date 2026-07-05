using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using ThemeEditor.parser;
using Windows.Storage;
using static System.Windows.Forms.DataFormats;

namespace ThemeEditor.colors;


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

    public void GenerateContrastFromNormal() {
        Light.GenerateContrastFromNormal(DayNightMode.Light);
        Dark.GenerateContrastFromNormal(DayNightMode.Dark);
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