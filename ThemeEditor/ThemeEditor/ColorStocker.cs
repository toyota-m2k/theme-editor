using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

namespace ThemeEditor;

public record StockedColorPair(Color Background, Color Foreground) {
    public void ApplyTo(ColorPair pair) {
        pair.Background.Value = new NamedColor(pair.Background.Value.Name, Background);
        pair.Foreground.Value = new NamedColor(pair.Foreground.Value.Name, Foreground);
    }
    public static StockedColorPair CreateFrom(ColorPair pair) {
        return new StockedColorPair(pair.Background.Value.Color, pair.Foreground.Value.Color);
    }
}

public record StockedTheme(
    StockedColorPair Base,
    StockedColorPair Container,
    StockedColorPair Fixed,
    StockedColorPair FixedDim,
    StockedColorPair? Inverse) {

    public void ApplyTo(ICommonTheme theme) {
        Base.ApplyTo(theme.Base);
        Container.ApplyTo(theme.Container);
        Fixed.ApplyTo(theme.Fixed);
        FixedDim.ApplyTo(theme.FixedDim);
        if (Inverse != null && theme is IPrimaryTheme primary) {
            Inverse.ApplyTo(primary.Inverse);
        }
    }

    public static StockedTheme CreateFrom(ICommonTheme theme) {
        return new StockedTheme(
            StockedColorPair.CreateFrom(theme.Base),
            StockedColorPair.CreateFrom(theme.Container),
            StockedColorPair.CreateFrom(theme.Fixed),
            StockedColorPair.CreateFrom(theme.FixedDim),
            theme is IPrimaryTheme primary ? StockedColorPair.CreateFrom(primary.Inverse) : null);
    }
}

public record StockedContrastTheme(
    StockedTheme Normal,
    StockedTheme Medium,
    StockedTheme High) {

    public void ApplyTo(AndroidContrastColorTheme theme, CommonThemeType type) {
        Normal.ApplyTo(theme.NormalTheme.CommonThemeOf(type));
        Medium.ApplyTo(theme.MediumTheme.CommonThemeOf(type));
        High.ApplyTo(theme.HighTheme.CommonThemeOf(type));
    }

    public static StockedContrastTheme CreateFrom(AndroidContrastColorTheme theme, CommonThemeType type) {
        return new StockedContrastTheme(
            StockedTheme.CreateFrom(theme.NormalTheme.CommonThemeOf(type)),
            StockedTheme.CreateFrom(theme.MediumTheme.CommonThemeOf(type)),
            StockedTheme.CreateFrom(theme.HighTheme.CommonThemeOf(type)));
    }
}

public record StockedThemeSet(
    StockedContrastTheme Light,
    StockedContrastTheme Dark) {

    public void ApplyTo(AndroidColorThemeSet themeSet, CommonThemeType type) {
        Light.ApplyTo(themeSet.Light, type);
        Dark.ApplyTo(themeSet.Dark, type);
    }

    public static StockedThemeSet CreateFrom(AndroidColorThemeSet themeSet, CommonThemeType type) {
        return new StockedThemeSet(
            StockedContrastTheme.CreateFrom(themeSet.Light, type),
            StockedContrastTheme.CreateFrom(themeSet.Dark, type));
    }
}



public record StockItem(
    string Id,
    string Title,
    CommonThemeType Type,
    DateTime CreatedAt,
    StockedThemeSet ThemeSet) {

    public void ApplyTo(AndroidColorThemeSet themeSet) {
        ThemeSet.ApplyTo(themeSet, Type);
    }

    public void ApplyTo(AndroidColorThemeSet themeSet, CommonThemeType type) {
        ThemeSet.ApplyTo(themeSet, type);
    }

    public static StockItem CreateFrom(AndroidColorThemeSet themeSet, CommonThemeType type, string title) {
        return new StockItem(
            Guid.NewGuid().ToString(),
            title,
            type,
            DateTime.Now,
            StockedThemeSet.CreateFrom(themeSet, type));
    }
}

public record StockFile(int Version, List<StockItem> Items);

public class ColorJsonConverter : JsonConverter<Color> {
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        if (string.IsNullOrEmpty(value)) {
            return Color.Empty;
        }
        return NamedColor.ColorFromHexString(value);
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options) {
        if (value.IsEmpty) {
            writer.WriteStringValue("");
            return;
        }
        writer.WriteStringValue($"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}");
    }
}

public class ColorStocker {
    private const int CurrentVersion = 1;
    private const string FileName = "color-stock.json";

    private static readonly JsonSerializerOptions JsonOptions = new() {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = {
            new ColorJsonConverter(),
            new JsonStringEnumConverter(),
        },
    };

    public ObservableCollection<StockItem> StockedThemes { get; } = new();

    private static string FilePath => Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);

    public void Load() {
        StockedThemes.Clear();
        var path = FilePath;
        if (!File.Exists(path)) {
            return;
        }
        try {
            var json = File.ReadAllText(path);
            var file = JsonSerializer.Deserialize<StockFile>(json, JsonOptions);
            if (file?.Items == null) {
                return;
            }
            foreach (var item in file.Items) {
                StockedThemes.Add(item);
            }
        }
        catch (Exception e) {
            System.Diagnostics.Debug.WriteLine($"ColorStocker.Load failed: {e.Message}");
        }
    }

    private void Save() {
        try {
            var path = FilePath;
            var tmp = path + ".tmp";
            var file = new StockFile(CurrentVersion, StockedThemes.ToList());
            File.WriteAllText(tmp, JsonSerializer.Serialize(file, JsonOptions));
            File.Move(tmp, path, overwrite: true);
        }
        catch (Exception e) {
            System.Diagnostics.Debug.WriteLine($"ColorStocker.Save failed: {e.Message}");
        }
    }

    public StockItem Add(AndroidColorThemeSet themeSet, CommonThemeType type, string title) {
        var item = StockItem.CreateFrom(themeSet, type, title);
        StockedThemes.Add(item);
        Save();
        return item;
    }

    public void Update(StockItem oldItem, StockItem newItem) {
        var index = StockedThemes.IndexOf(oldItem);
        if (index < 0) {
            return;
        }
        StockedThemes[index] = newItem;
        Save();
    }

    public void Remove(StockItem item) {
        if (StockedThemes.Remove(item)) {
            Save();
        }
    }
}

