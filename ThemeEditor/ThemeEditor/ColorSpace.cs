using System;
using System.Drawing;

namespace ThemeEditor;

/// <summary>
/// sRGB <-> CIELAB (D65) conversion and per-role contrast "delta transfer".
/// Used to derive Medium/High contrast colors from an edited Normal color by
/// re-applying the original theme's Normal->Medium / Normal->High difference.
/// </summary>
public static class ColorSpace {
    // D65 reference white.
    private const double Xn = 95.047;
    private const double Yn = 100.0;
    private const double Zn = 108.883;

    public readonly record struct Lab(double L, double A, double B);

    public static Lab RgbToLab(Color color) {
        // sRGB (0..255) -> linear (0..1)
        double r = GammaExpand(color.R / 255.0);
        double g = GammaExpand(color.G / 255.0);
        double b = GammaExpand(color.B / 255.0);

        // linear RGB -> XYZ (D65), scaled to 0..100
        double x = (r * 0.4124 + g * 0.3576 + b * 0.1805) * 100.0;
        double y = (r * 0.2126 + g * 0.7152 + b * 0.0722) * 100.0;
        double z = (r * 0.0193 + g * 0.1192 + b * 0.9505) * 100.0;

        double fx = LabF(x / Xn);
        double fy = LabF(y / Yn);
        double fz = LabF(z / Zn);

        return new Lab(116.0 * fy - 16.0, 500.0 * (fx - fy), 200.0 * (fy - fz));
    }

    public static Color LabToRgb(Lab lab) {
        double fy = (lab.L + 16.0) / 116.0;
        double fx = fy + lab.A / 500.0;
        double fz = fy - lab.B / 200.0;

        double x = LabFInv(fx) * Xn;
        double y = LabFInv(fy) * Yn;
        double z = LabFInv(fz) * Zn;

        // XYZ (0..100) -> linear RGB
        x /= 100.0; y /= 100.0; z /= 100.0;
        double r = x * 3.2406 + y * -1.5372 + z * -0.4986;
        double g = x * -0.9689 + y * 1.8758 + z * 0.0415;
        double b = x * 0.0557 + y * -0.2040 + z * 1.0570;

        return Color.FromArgb(ToByte(GammaCompress(r)), ToByte(GammaCompress(g)), ToByte(GammaCompress(b)));
    }

    /// <summary>
    /// Returns <paramref name="current"/> shifted by the Lab-space difference
    /// (origTo - origFrom). Lightness is clamped to [0,100]; RGB channels are
    /// clamped on the way back so out-of-gamut results stay valid.
    /// </summary>
    public static Color Transfer(Color current, Color origFrom, Color origTo) {
        var cur = RgbToLab(current);
        var from = RgbToLab(origFrom);
        var to = RgbToLab(origTo);

        double l = Math.Clamp(cur.L + (to.L - from.L), 0.0, 100.0);
        double a = cur.A + (to.A - from.A);
        double b = cur.B + (to.B - from.B);

        return LabToRgb(new Lab(l, a, b));
    }

    /// <summary>
    /// Returns <paramref name="color"/> with its CIELAB lightness (L*) shifted by
    /// <paramref name="deltaL"/>, keeping a*/b* (hue &amp; colorfulness). L* is clamped
    /// to [0,100] and RGB channels are clamped on the way back. Since HCT tone == L*,
    /// this applies a Material-style contrast tone shift while preserving the color.
    /// </summary>
    public static Color ShiftLightness(Color color, double deltaL) {
        var lab = RgbToLab(color);
        double l = Math.Clamp(lab.L + deltaL, 0.0, 100.0);
        return LabToRgb(new Lab(l, lab.A, lab.B));
    }

    private static double GammaExpand(double c) {
        return c <= 0.04045 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
    }

    private static double GammaCompress(double c) {
        c = Math.Clamp(c, 0.0, 1.0);
        return c <= 0.0031308 ? c * 12.92 : 1.055 * Math.Pow(c, 1.0 / 2.4) - 0.055;
    }

    private static double LabF(double t) {
        const double delta = 6.0 / 29.0;
        return t > delta * delta * delta ? Math.Cbrt(t) : t / (3.0 * delta * delta) + 4.0 / 29.0;
    }

    private static double LabFInv(double t) {
        const double delta = 6.0 / 29.0;
        return t > delta ? t * t * t : 3.0 * delta * delta * (t - 4.0 / 29.0);
    }

    private static int ToByte(double v) {
        return (int)Math.Round(Math.Clamp(v, 0.0, 1.0) * 255.0);
    }
}
