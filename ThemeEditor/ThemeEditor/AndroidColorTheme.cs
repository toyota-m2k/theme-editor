using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using static ThemeEditor.AndroidColorTheme;

namespace ThemeEditor;

public interface ICommonTheme {
    ColorPair Base { get; }
    ColorPair Container { get; }
    ColorPair Fixed { get; }
    ColorPair FixedDim { get; }
}
public interface IPrimaryTheme : ICommonTheme {
    ColorPair Inverse { get; }
}

public class ColorPair {
    public ReactiveProperty<NamedColor> Background { get; }
    public ReactiveProperty<NamedColor> Foreground { get; }
    public ColorPair(ReactiveProperty<NamedColor> background, ReactiveProperty<NamedColor> foreground) {
        Background = background;
        Foreground = foreground;
    }

    public bool IsEmpty => Background.Value.IsEmpty && Foreground.Value.IsEmpty;
    public ReadOnlyReactiveProperty<bool> IsEmptyObservable => Background.CombineLatest(Foreground, (b, f) => b.IsEmpty && f.IsEmpty).ToReadOnlyReactiveProperty();
}



public class AndroidColorTheme {
    // primary
    private ReactiveProperty<NamedColor> primary { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> primaryContainer { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> inversePrimary { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> primaryFixed { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> primaryFixedDim { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> onPrimary { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> onPrimaryContainer { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> onPrimaryFixed { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> onPrimaryFixedVariant { get; } = new(NamedColor.Empty);

    //Secondary --

    private ReactiveProperty<NamedColor> secondary { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> secondaryContainer { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> secondaryFixed { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> secondaryFixedDim { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> onSecondary { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> onSecondaryContainer { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> onSecondaryFixed { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> onSecondaryFixedVariant { get; } = new(NamedColor.Empty);


    //Tertiary --
    private ReactiveProperty<NamedColor> tertiary { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> tertiaryContainer { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> tertiaryFixed { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> tertiaryFixedDim { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> onTertiary { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> onTertiaryContainer { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> onTertiaryFixed { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> onTertiaryFixedVariant { get; } = new(NamedColor.Empty);

    //Surface --
    private ReactiveProperty<NamedColor> surface { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> surfaceVariant { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> inverseSurface { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> inverseOnSurface { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> surfaceDim { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> surfaceBright { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> surfaceContainerLowest { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> surfaceContainerLow { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> surfaceContainer { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> surfaceContainerHigh { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> surfaceContainerHighest { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> onSurface { get; } = new(NamedColor.Empty);
    private ReactiveProperty<NamedColor> onSurfaceVariant { get; } = new(NamedColor.Empty);

    //public abstract class Common {
    //    public ColorPair Base { get; set; }
    //    public ColorPair Container { get; set; }
    //    public ColorPair Fixed { get; set; }
    //    public ColorPair FixedDim { get; set; }
    //    public Common(ColorPair @base, ColorPair container, ColorPair @fixed, ColorPair fixedDim) {
    //        Base = @base;
    //        Container = container;
    //        Fixed = @fixed;
    //        FixedDim = fixedDim;
    //    }
    //}

    public class PrimaryTheme : IPrimaryTheme {
        public ColorPair Base { get; }
        public ColorPair Container { get; }
        public ColorPair Fixed { get; }
        public ColorPair FixedDim { get; }
        public ColorPair Inverse { get; }

        public PrimaryTheme(AndroidColorTheme p) { 
            Base = new ColorPair(p.primary, p.onPrimary);
            Container = new ColorPair(p.primaryContainer, p.onPrimaryContainer);
            Fixed = new ColorPair(p.primaryFixed, p.onPrimaryFixed);
            FixedDim = new ColorPair(p.primaryFixedDim, p.onPrimaryFixedVariant);
            Inverse = new ColorPair(p.inversePrimary, p.onPrimaryContainer);
        }
        public bool IsEmpty => Base.IsEmpty || Container.IsEmpty || Fixed.IsEmpty || FixedDim.IsEmpty || Inverse.IsEmpty;
        public ReadOnlyReactiveProperty<bool> IsEmptyObservable => Base.IsEmptyObservable.CombineLatest(Container.IsEmptyObservable, Fixed.IsEmptyObservable, FixedDim.IsEmptyObservable, Inverse.IsEmptyObservable, (b, c, f, fd, i) => b && c && f && fd && i).ToReadOnlyReactiveProperty();
    }

    public class SecondaryTheme : ICommonTheme {
        public ColorPair Base { get; }
        public ColorPair Container { get; }
        public ColorPair Fixed { get; }
        public ColorPair FixedDim { get; }
        public SecondaryTheme(AndroidColorTheme p) {
            Base = new ColorPair(p.secondary, p.onSecondary);
            Container = new ColorPair(p.secondaryContainer, p.onSecondaryContainer);
            Fixed = new ColorPair(p.secondaryFixed, p.onSecondaryFixed);
            FixedDim = new ColorPair(p.secondaryFixedDim, p.onSecondaryFixedVariant);
        }

        public bool IsEmpty => Base.IsEmpty || Container.IsEmpty || Fixed.IsEmpty || FixedDim.IsEmpty;
        public ReadOnlyReactiveProperty<bool> IsEmptyObservable => Base.IsEmptyObservable.CombineLatest(Container.IsEmptyObservable, Fixed.IsEmptyObservable, FixedDim.IsEmptyObservable, (b, c, f, fd) => b && c && f && fd).ToReadOnlyReactiveProperty();
    }

    public class TertiaryTheme : ICommonTheme {
        public ColorPair Base { get; }
        public ColorPair Container { get; }
        public ColorPair Fixed { get; }
        public ColorPair FixedDim { get; }
        public TertiaryTheme(AndroidColorTheme p) {
            Base = new ColorPair(p.tertiary, p.onTertiary);
            Container = new ColorPair(p.tertiaryContainer, p.onTertiaryContainer);
            Fixed = new ColorPair(p.tertiaryFixed, p.onTertiaryFixed);
            FixedDim = new ColorPair(p.tertiaryFixedDim, p.onTertiaryFixedVariant);
        }

        public bool IsEmpty => Base.Background.Value.IsEmpty && Base.Foreground.Value.IsEmpty && Container.Background.Value.IsEmpty && Container.Foreground.Value.IsEmpty && Fixed.Background.Value.IsEmpty && Fixed.Foreground.Value.IsEmpty && FixedDim.Background.Value.IsEmpty && FixedDim.Foreground.Value.IsEmpty;
        public ReadOnlyReactiveProperty<bool> IsEmptyObservable => Base.IsEmptyObservable.CombineLatest(Container.IsEmptyObservable, Fixed.IsEmptyObservable, FixedDim.IsEmptyObservable, (b, c, f, fd) => b && c && f && fd).ToReadOnlyReactiveProperty();
    }

    public class SurfaceTheme {
        public ColorPair Base { get; set; }
        public ColorPair Variant { get; set; }
        public ColorPair Inverse { get; set; }
        public ColorPair Dim { get; set; }
        public ColorPair Bright { get; set; }
        public ColorPair ContainerLowest { get; set; }
        public ColorPair ContainerLow { get; set; }
        public ColorPair Container { get; set; }
        public ColorPair ContainerHigh { get; set; }
        public ColorPair ContainerHighest { get; set; }

        public SurfaceTheme(AndroidColorTheme p) {
            Base = new ColorPair(p.surface, p.onSurface);
            Variant = new ColorPair(p.surfaceVariant, p.onSurfaceVariant);
            Inverse = new ColorPair(p.inverseSurface, p.inverseOnSurface);
            Dim = new ColorPair(p.surfaceDim, p.onSurface);
            Bright = new ColorPair(p.surfaceBright, p.onSurface);
            ContainerLowest = new ColorPair(p.surfaceContainerLowest, p.onSurface);
            ContainerLow = new ColorPair(p.surfaceContainerLow, p.onSurface);
            Container = new ColorPair(p.surfaceContainer, p.onSurface);
            ContainerHigh = new ColorPair(p.surfaceContainerHigh, p.onSurface);
            ContainerHighest = new ColorPair(p.surfaceContainerHighest, p.onSurface);
        }

        public bool IsEmpty => Base.IsEmpty || Variant.IsEmpty || Inverse.IsEmpty || Dim.IsEmpty || Bright.IsEmpty || ContainerLowest.IsEmpty || ContainerLow.IsEmpty || Container.IsEmpty || ContainerHigh.IsEmpty || ContainerHighest.IsEmpty;
        public ReadOnlyReactiveProperty<bool> IsEmptyObservable => Base.IsEmptyObservable.CombineLatest(Variant.IsEmptyObservable, Inverse.IsEmptyObservable, Dim.IsEmptyObservable, Bright.IsEmptyObservable, ContainerLowest.IsEmptyObservable, ContainerLow.IsEmptyObservable, Container.IsEmptyObservable, ContainerHigh.IsEmptyObservable, ContainerHighest.IsEmptyObservable, (b, v, i, d, br, cl, clow, c, ch, chigh) => b && v && i && d && br && cl && clow && c && ch && chigh).ToReadOnlyReactiveProperty();
    }

    public PrimaryTheme Primary;
    public SecondaryTheme Secondary;
    public TertiaryTheme Tertiary;
    public SurfaceTheme Surface;
    private AndroidColorTheme(List<NamedColor> colors) {
        primary.Value = colors.FirstOrDefault(it => it.Name == "primary") ?? NamedColor.Empty;
        primaryContainer.Value = colors.FirstOrDefault(it => it.Name == "primaryContainer") ?? NamedColor.Empty;
        inversePrimary.Value = colors.FirstOrDefault(it => it.Name == "inversePrimary") ?? NamedColor.Empty;
        primaryFixed.Value = colors.FirstOrDefault(it => it.Name == "primaryFixed") ?? NamedColor.Empty;
        primaryFixedDim.Value = colors.FirstOrDefault(it => it.Name == "primaryFixedDim") ?? NamedColor.Empty;
        onPrimary.Value = colors.FirstOrDefault(it => it.Name == "onPrimary") ?? NamedColor.Empty;
        onPrimaryContainer.Value = colors.FirstOrDefault(it => it.Name == "onPrimaryContainer") ?? NamedColor.Empty;
        onPrimaryFixed.Value = colors.FirstOrDefault(it => it.Name == "onPrimaryFixed") ?? NamedColor.Empty;
        onPrimaryFixedVariant.Value = colors.FirstOrDefault(it => it.Name == "onPrimaryFixedVariant") ?? NamedColor.Empty;
        secondary.Value = colors.FirstOrDefault(it => it.Name == "secondary") ?? NamedColor.Empty;
        secondaryContainer.Value = colors.FirstOrDefault(it => it.Name == "secondaryContainer") ?? NamedColor.Empty;
        secondaryFixed.Value = colors.FirstOrDefault(it => it.Name == "secondaryFixed") ?? NamedColor.Empty;
        secondaryFixedDim.Value = colors.FirstOrDefault(it => it.Name == "secondaryFixedDim") ?? NamedColor.Empty;
        onSecondary.Value = colors.FirstOrDefault(it => it.Name == "onSecondary") ?? NamedColor.Empty;
        onSecondaryContainer.Value = colors.FirstOrDefault(it => it.Name == "onSecondaryContainer") ?? NamedColor.Empty;
        onSecondaryFixed.Value = colors.FirstOrDefault(it => it.Name == "onSecondaryFixed") ?? NamedColor.Empty;
        onSecondaryFixedVariant.Value = colors.FirstOrDefault(it => it.Name == "onSecondaryFixedVariant") ?? NamedColor.Empty;
        tertiary.Value = colors.FirstOrDefault(it => it.Name == "tertiary") ?? NamedColor.Empty;
        tertiaryContainer.Value = colors.FirstOrDefault(it => it.Name == "tertiaryContainer") ?? NamedColor.Empty;
        tertiaryFixed.Value = colors.FirstOrDefault(it => it.Name == "tertiaryFixed") ?? NamedColor.Empty;
        tertiaryFixedDim.Value = colors.FirstOrDefault(it => it.Name == "tertiaryFixedDim") ?? NamedColor.Empty;
        onTertiary.Value = colors.FirstOrDefault(it => it.Name == "onTertiary") ?? NamedColor.Empty;
        onTertiaryContainer.Value = colors.FirstOrDefault(it => it.Name == "onTertiaryContainer") ?? NamedColor.Empty;
        onTertiaryFixed.Value = colors.FirstOrDefault(it => it.Name == "onTertiaryFixed") ?? NamedColor.Empty;
        onTertiaryFixedVariant.Value = colors.FirstOrDefault(it => it.Name == "onTertiaryFixedVariant") ?? NamedColor.Empty;
        surface.Value = colors.FirstOrDefault(it => it.Name == "surface") ?? NamedColor.Empty;
        surfaceVariant.Value = colors.FirstOrDefault(it => it.Name == "surfaceVariant") ?? NamedColor.Empty;
        inverseSurface.Value = colors.FirstOrDefault(it => it.Name == "inverseSurface") ?? NamedColor.Empty;
        inverseOnSurface.Value = colors.FirstOrDefault(it => it.Name == "inverseOnSurface") ?? NamedColor.Empty;
        surfaceDim.Value = colors.FirstOrDefault(it => it.Name == "surfaceDim") ?? NamedColor.Empty;
        surfaceBright.Value = colors.FirstOrDefault(it => it.Name == "surfaceBright") ?? NamedColor.Empty;
        surfaceContainerLowest.Value = colors.FirstOrDefault(it => it.Name == "surfaceContainerLowest") ?? NamedColor.Empty;
        surfaceContainerLow.Value = colors.FirstOrDefault(it => it.Name == "surfaceContainerLow") ?? NamedColor.Empty;
        surfaceContainer.Value = colors.FirstOrDefault(it => it.Name == "surfaceContainer") ?? NamedColor.Empty;
        surfaceContainerHigh.Value = colors.FirstOrDefault(it => it.Name == "surfaceContainerHigh") ?? NamedColor.Empty;
        surfaceContainerHighest.Value = colors.FirstOrDefault(it => it.Name == "surfaceContainerHighest") ?? NamedColor.Empty;
        onSurface.Value = colors.FirstOrDefault(it => it.Name == "onSurface") ?? NamedColor.Empty;
        onSurfaceVariant.Value = colors.FirstOrDefault(it => it.Name == "onSurfaceVariant") ?? NamedColor.Empty;
        Primary = new(this);
        Secondary = new(this);
        Tertiary = new(this);
        Surface = new(this);
    }

    public NamedColor? GetColorByName(string name) {
        switch (name) {
            case "primary": return primary.Value;
            case "primaryContainer": return primaryContainer.Value;
            case "inversePrimary": return inversePrimary.Value;
            case "primaryFixed": return primaryFixed.Value;
            case "primaryFixedDim": return primaryFixedDim.Value;
            case "onPrimary": return onPrimary.Value;
            case "onPrimaryContainer": return onPrimaryContainer.Value;
            case "onPrimaryFixed": return onPrimaryFixed.Value;
            case "onPrimaryFixedVariant": return onPrimaryFixedVariant.Value;
            case "secondary": return secondary.Value;
            case "secondaryContainer": return secondaryContainer.Value;
            case "secondaryFixed": return secondaryFixed.Value;
            case "secondaryFixedDim": return secondaryFixedDim.Value;
            case "onSecondary": return onSecondary.Value;
            case "onSecondaryContainer": return onSecondaryContainer.Value;
            case "onSecondaryFixed": return onSecondaryFixed.Value;
            case "onSecondaryFixedVariant": return onSecondaryFixedVariant.Value;
            case "tertiary": return tertiary.Value;
            case "tertiaryContainer": return tertiaryContainer.Value;
            case "tertiaryFixed": return tertiaryFixed.Value;
            case "tertiaryFixedDim": return tertiaryFixedDim.Value;
            case "onTertiary": return onTertiary.Value;
            case "onTertiaryContainer": return onTertiaryContainer.Value;
            case "onTertiaryFixed": return onTertiaryFixed.Value;
            case "onTertiaryFixedVariant": return onTertiaryFixedVariant.Value;
            case "surface": return surface.Value;
            case "surfaceVariant": return surfaceVariant.Value;
            case "inverseSurface": return inverseSurface.Value;
            case "inverseOnSurface": return inverseOnSurface.Value;
            case "surfaceDim": return surfaceDim.Value;
            case "surfaceBright": return surfaceBright.Value;
            case "surfaceContainerLowest": return surfaceContainerLowest.Value;
            case "surfaceContainerLow": return surfaceContainerLow.Value;
            case "surfaceContainer": return surfaceContainer.Value;
            case "surfaceContainerHigh": return surfaceContainerHigh.Value;
            case "surfaceContainerHighest": return surfaceContainerHighest.Value;
            case "onSurface": return onSurface.Value;
            case "onSurfaceVariant": return onSurfaceVariant.Value;
            default: return null;
        }
    }

    public void SetColorByName(string name, NamedColor color) {
        switch (name) {
            case "primary": primary.Value = color; break;
            case "primaryContainer": primaryContainer.Value = color; break;
            case "inversePrimary": inversePrimary.Value = color; break;
            case "primaryFixed": primaryFixed.Value = color; break;
            case "primaryFixedDim": primaryFixedDim.Value = color; break;
            case "onPrimary": onPrimary.Value = color; break;
            case "onPrimaryContainer": onPrimaryContainer.Value = color; break;
            case "onPrimaryFixed": onPrimaryFixed.Value = color; break;
            case "onPrimaryFixedVariant": onPrimaryFixedVariant.Value = color; break;
            case "secondary": secondary.Value = color; break;
            case "secondaryContainer": secondaryContainer.Value = color; break;
            case "secondaryFixed": secondaryFixed.Value = color; break;
            case "secondaryFixedDim": secondaryFixedDim.Value = color; break;
            case "onSecondary": onSecondary.Value = color; break;
            case "onSecondaryContainer": onSecondaryContainer.Value = color; break;
            case "onSecondaryFixed": onSecondaryFixed.Value = color; break;
            case "onSecondaryFixedVariant": onSecondaryFixedVariant.Value = color; break;
            case "tertiary": tertiary.Value = color; break;
            case "tertiaryContainer": tertiaryContainer.Value = color; break;
            case "tertiaryFixed": tertiaryFixed.Value = color; break;
            case "tertiaryFixedDim": tertiaryFixedDim.Value = color; break;
            case "onTertiary": onTertiary.Value = color; break;
            case "onTertiaryContainer": onTertiaryContainer.Value = color; break;
            case "onTertiaryFixed": onTertiaryFixed.Value = color; break;
            case "onTertiaryFixedVariant": onTertiaryFixedVariant.Value = color; break;
            case "surface": surface.Value = color; break;
            case "surfaceVariant": surfaceVariant.Value = color; break;
            case "inverseSurface": inverseSurface.Value = color; break;
            case "inverseOnSurface": inverseOnSurface.Value = color; break;
            case "surfaceDim": surfaceDim.Value = color; break;
            case "surfaceBright": surfaceBright.Value = color; break;
            case "surfaceContainerLowest": surfaceContainerLowest.Value = color; break;
            case "surfaceContainerLow": surfaceContainerLow.Value = color; break;
            case "surfaceContainer": surfaceContainer.Value = color; break;
            case "surfaceContainerHigh": surfaceContainerHigh.Value = color; break;
            case "surfaceContainerHighest": surfaceContainerHighest.Value = color; break;
            case "onSurface": onSurface.Value = color; break;
            case "onSurfaceVariant": onSurfaceVariant.Value = color; break;
            default: break;
        }
    }

    public void ExchangeColors(ExchangeColorTarget target) {
        switch (target) {
            case ExchangeColorTarget.Primary_Secondary:
                ExchangeCommonTheme(Primary, Secondary);
                break;
            case ExchangeColorTarget.Primary_Tertiary:
                ExchangeCommonTheme(Primary, Tertiary);
                break;
            case ExchangeColorTarget.Secondary_Tertiary:
                ExchangeCommonTheme(Secondary, Tertiary);
                break;
            default:
                throw new InvalidEnumArgumentException();
        }
    }

    public ICommonTheme CommonThemeOf(CommonThemeType type) {
        return type switch {
            CommonThemeType.Primary => Primary,
            CommonThemeType.Secondary => Secondary,
            CommonThemeType.Tertiary => Tertiary,
            _ => throw new InvalidEnumArgumentException(),
        };
    }

    private void ExchangeCommonTheme(ICommonTheme t1, ICommonTheme t2) {
        if (object.ReferenceEquals(t1, t2)) {
            return;
        }
        void exchangeNamedColor(ReactiveProperty<NamedColor> n1, ReactiveProperty<NamedColor> n2) {
            var temp = n1.Value;
            n1.Value = n2.Value;
            n2.Value = temp;
        }
        void exchangeColorPair(ColorPair p1, ColorPair p2) {
            exchangeNamedColor(p1.Background, p2.Background);
            exchangeNamedColor(p1.Foreground, p2.Foreground);
        }
        exchangeColorPair(t1.Base, t2.Base);
        exchangeColorPair(t1.Container, t2.Container);
        exchangeColorPair(t1.Fixed, t2.Fixed);
        exchangeColorPair(t1.FixedDim, t2.FixedDim);
    }

    public static AndroidColorTheme NormalTheme(AndroidColors colors) {
        return new AndroidColorTheme(colors.Normal);
    }
    public static AndroidColorTheme MediumTheme(AndroidColors colors) {
        return new AndroidColorTheme(colors.Medium);
    }
    public static AndroidColorTheme HighTheme(AndroidColors colors) {
        return new AndroidColorTheme(colors.High);
    }
    public static AndroidColorTheme? Create(AndroidColors? colors, ColorContrast contrast) {
        if(colors == null) {
            return null;
        }
        return contrast switch {
            ColorContrast.Normal => NormalTheme(colors),
            ColorContrast.Medium => MediumTheme(colors),
            ColorContrast.High => HighTheme(colors),
            _ => throw new InvalidEnumArgumentException(),
        };
    }
}