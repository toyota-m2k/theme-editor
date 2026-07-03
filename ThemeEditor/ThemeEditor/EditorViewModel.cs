using Reactive.Bindings;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace ThemeEditor;

class EditorViewModel {
    public static ReactiveCommand<NamedColor> BackgroundColorChanged { get; } = new();

    //public ReactiveProperty<StorageFile?> InputFile { get; } = new();
    public ReactiveCommand OpenFileCommand { get; } = new();
    public ReactiveCommand SaveFileCommand { get; } = new();
    public ReactiveCommand CopyThemeCommand { get; } = new();
    public ReactiveCommand ExchangeColorCommand { get; } = new();

    //public ReactiveProperty<AndroidColors?> Colors { get; } = new();
    public ReactiveProperty<AndroidColorThemeSet> ThemeSet { get; } = new();

    public ReactiveProperty<ColorContrast> Contrast { get; } = new(ColorContrast.Normal);
    public ReactiveProperty<DayNightMode> DayNight { get; } = new(DayNightMode.Light);
    public ReadOnlyReactiveProperty<AndroidColorTheme?> Theme { get; }
    public ReadOnlyReactiveProperty<bool> IsThemeAvailable { get; }
    //public ReactiveProperty<bool> IsThemeAvailable = new(false);
    public ReactiveProperty<bool> EnableInterlockColors { get; } = new(true);

    public EditorViewModel() {
        Theme = ThemeSet.CombineLatest(DayNight, Contrast, (themeSet, daynight, contrast) => themeSet?.ThemeOf(daynight, contrast)).ToReadOnlyReactiveProperty();
        IsThemeAvailable = Theme.Select(it => it != null).ToReadOnlyReactiveProperty();
        BackgroundColorChanged.Subscribe(HandleInterlockColor);
    }

    private void HandleInterlockColor(NamedColor newBackgroundColor) {
        if (!EnableInterlockColors.Value) return;
        var interlocker = InterlockColors.Create(ThemeSet.Value, DayNight.Value, Contrast.Value);
        interlocker?.Apply(newBackgroundColor);
    }

    //private void ChangeColor(ReactiveProperty<NamedColor> target, NamedColor color) {
    //    target.Value = target.Value.NewColor(color.Color);
    //}

    //private void HandleInterlockLightColor(NamedColor newBackgroundColor) {
    //    var lightTheme = ThemeSet.Value.Light?.ThemeOf(Contrast.Value);
    //    if (lightTheme == null) return;
    //    var darkTheme = ThemeSet.Value.Dark?.ThemeOf(Contrast.Value);

    //    AndroidColorTheme.ICommon lightTarget;
    //    AndroidColorTheme.ICommon? darkTarget;
    //    switch (newBackgroundColor.Name) {
    //        case "primary":
    //            lightTarget = lightTheme.Primary;
    //            darkTarget = darkTheme?.Primary;
    //            break;
    //        case "secondary":
    //            lightTarget = lightTheme.Secondary;
    //            darkTarget = darkTheme?.Secondary;
    //            break;
    //        case "tertiary":
    //            lightTarget = lightTheme.Tertiary;
    //            darkTarget = darkTheme?.Tertiary;
    //            break;
    //        default: return;

    //    }

    //    ChangeColor(lightTarget.Container.Foreground, newBackgroundColor);
    //    ChangeColor(lightTarget.Fixed.Foreground, newBackgroundColor);
    //    ChangeColor(lightTarget.FixedDim.Foreground, newBackgroundColor);

    //    if (darkTarget != null) {
    //        ChangeColor(darkTarget.Container.Background, newBackgroundColor);
    //        ChangeColor(darkTarget.Base.Foreground, newBackgroundColor);
    //        ChangeColor(darkTarget.Fixed.Foreground, newBackgroundColor);
    //        ChangeColor(darkTarget.FixedDim.Foreground, newBackgroundColor);
    //    }

    //}
    //private void HandleInterlockDarkColor(NamedColor newBackgroundColor) {

    //    //var theme = Theme.Value;
    //    //if (theme == null) return;
    //    var lightTheme = ThemeSet.Value.Light?.ThemeOf(Contrast.Value);
    //    if (lightTheme == null) return;
    //    AndroidColorTheme.ICommon target;

    //    switch (newBackgroundColor.Name) {
    //        case "primaryContainer":
    //            target = lightTheme.Primary;
    //            break;
    //        case "secondaryContainer":
    //            target = lightTheme.Secondary;
    //            break;
    //        case "tertialyContainer":
    //            target = lightTheme.Tertiary;
    //            break;
    //        default:
    //            return;
    //    }
    //    ChangeColor(target.Base.Background, newBackgroundColor);
    //    HandleInterlockLightColor(target.Base.Background.Value);
    //}

}
