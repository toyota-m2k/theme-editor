using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThemeEditor;

internal class InterlockColors {
    AndroidColorTheme Light;
    AndroidColorTheme Dark;
    DayNightMode DayNightMode;


    public static InterlockColors? Create(AndroidColorThemeSet themeSet, DayNightMode mode, ColorContrast contrast) {
        if (themeSet.Light == null || themeSet.Dark == null) {
            return null;
        }
        return new InterlockColors(themeSet.Light.ThemeOf(contrast), themeSet.Dark.ThemeOf(contrast), mode, contrast);
    }
    public InterlockColors(AndroidColorTheme light, AndroidColorTheme dark, DayNightMode mode, ColorContrast contrast) {
        Light = light;
        Dark = dark;
        DayNightMode = mode;
    }
    public void Apply(NamedColor newColor) {
        if (DayNightMode== DayNightMode.Light) {
            ApplyOnLightMode(newColor);
        }
        else {
            ApplyOnDarkMode(newColor);
        }
    }

    private void ChangeColor(ReactiveProperty<NamedColor> target, NamedColor color) {
        target.Value = target.Value.NewColor(color.Color);
    }

    void SetBaseBackgroundColor(NamedColor newColor, AndroidColorTheme.ICommonTheme lightTarget, AndroidColorTheme.ICommonTheme darkTarget) {
        ChangeColor(lightTarget.Base.Background, newColor);
        ChangeColor(lightTarget.Container.Foreground, newColor);
        ChangeColor(lightTarget.Fixed.Foreground, newColor);
        ChangeColor(lightTarget.FixedDim.Foreground, newColor);

        ChangeColor(darkTarget.Container.Background, newColor);
        ChangeColor(darkTarget.Base.Foreground, newColor);
        ChangeColor(darkTarget.Fixed.Foreground, newColor);
        ChangeColor(darkTarget.FixedDim.Foreground, newColor);
    }

    void SetContainerBackgroundColor(NamedColor newColor, AndroidColorTheme.ICommonTheme lightTarget, AndroidColorTheme.ICommonTheme darkTarget) {
        ChangeColor(lightTarget.Container.Background, newColor);
        ChangeColor(lightTarget.Fixed.Background, newColor);

        ChangeColor(darkTarget.Fixed.Background, newColor);
        ChangeColor(darkTarget.Container.Foreground, newColor);
    }

    void SetContainerFixedDimBackgroundColor(NamedColor newColor, AndroidColorTheme.ICommonTheme lightTarget, AndroidColorTheme.ICommonTheme darkTarget) {
        ChangeColor(lightTarget.FixedDim.Background, newColor);

        ChangeColor(darkTarget.Base.Background, newColor);
        ChangeColor(darkTarget.FixedDim.Background, newColor);
    }

    void ApplyOnLightMode(NamedColor newColor) {
        switch (newColor.Name) {
            case "primary":
                SetBaseBackgroundColor(newColor, Light.Primary, Dark.Primary);
                break;
            case "secondary":
                SetBaseBackgroundColor(newColor, Light.Secondary, Dark.Secondary);
                break;
            case "tertiary":
                SetBaseBackgroundColor(newColor, Light.Tertiary, Dark.Tertiary);
                break;

            case "primaryContainer":
            case "primaryFixed":
                SetContainerBackgroundColor(newColor, Light.Primary, Dark.Primary);
                break;
            case "secondaryContainer":
            case "secondaryFixed":
                SetContainerBackgroundColor(newColor, Light.Secondary, Dark.Secondary);
                break;
            case "tertiaryContainer":
            case "tertiaryFixed":
                SetContainerBackgroundColor(newColor, Light.Tertiary, Dark.Tertiary);
                break;

            case "primaryFixedDim":
                SetContainerFixedDimBackgroundColor(newColor, Light.Primary, Dark.Primary);
                break;
            case "secondaryFixedDim":
                SetContainerFixedDimBackgroundColor(newColor, Light.Secondary, Dark.Secondary);
                break;
            case "tertialyFixedDim":
                SetContainerFixedDimBackgroundColor(newColor, Light.Tertiary, Dark.Tertiary);
                break;
            default:
                return; // Unknown color name
        }
    }
    void ApplyOnDarkMode(NamedColor newColor) {
        switch (newColor.Name) {
            case "primaryFixed":
                ApplyOnLightMode(new NamedColor("primaryContainer", newColor.Color));
                break;
            case "secondaryFixed":
                ApplyOnLightMode(new NamedColor("secondaryContainer", newColor.Color));
                break;
            case "tertiaryFixed":
                ApplyOnLightMode(new NamedColor("tertiaryContainer", newColor.Color));
                break;

            case "primaryContainer":
                ApplyOnLightMode(new NamedColor("primary", newColor.Color));
                break;
            case "secondaryContainer":
                ApplyOnLightMode(new NamedColor("secondary", newColor.Color));
                break;
            case "tertiaryContainer":
                ApplyOnLightMode(new NamedColor("tertiary", newColor.Color));
                break;


            case "primary":
            case "primaryFixedDim":
                ApplyOnLightMode(new NamedColor("primaryFixedDim", newColor.Color));
                break;
            case "secondary":
            case "secondaryFixedDim":
                ApplyOnLightMode(new NamedColor("secondaryFixedDim", newColor.Color));
                break;
            case "tertiary":
            case "tertialyFixedDim":
                ApplyOnLightMode(new NamedColor("tertiaryFixedDim", newColor.Color));
                break;
            default:
                return; // Unknown color name
        }
    }
}
