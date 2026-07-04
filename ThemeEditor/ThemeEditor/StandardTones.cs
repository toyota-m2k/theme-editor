using System.Collections.Generic;

namespace ThemeEditor;

/// <summary>
/// Standard Material 3 tone (HCT tone == CIELAB L*) assignments per role, per
/// day/night mode, per contrast level (Normal / Medium / High).
///
/// These are HUE-INDEPENDENT constants: Material derives them by solving each
/// role's target contrast ratio against a (neutral) background, so they hold for
/// any theme regardless of its source color. Because they are baked in here (not
/// read from the imported file), Medium/High generation is stable across
/// edit -> save -> reload round-trips.
///
/// SOURCE / HOW TO REGENERATE: values below are the resolved tones of the Material
/// Color Utilities **2021** spec (verified: the Normal column reproduces the known
/// base tones exactly). The resolver lives in tools/resolve_material_tones.ps1 (a
/// faithful port of MCU getTone + ContrastCurve + ToneDeltaPair). To match a
/// different Theme Builder / spec version, either re-run that script for the target
/// spec, or read the L* of each role at each contrast level from a pristine Theme
/// Builder export (colors.xml with *_mediumContrast / *_highContrast), then replace
/// the numbers here. Only this file changes.
/// </summary>
public static class StandardTones {
    public readonly record struct ContrastTones(double Normal, double Medium, double High);

    // role -> (Light tones, Dark tones)
    private static readonly Dictionary<string, (ContrastTones Light, ContrastTones Dark)> Map = new() {
        // Primary family
        ["primary"]                 = (new(40, 23, 18),  new(80, 88, 95)),
        ["onPrimary"]               = (new(100, 100, 100), new(20, 15, 0)),
        ["primaryContainer"]        = (new(90, 46, 31),  new(30, 60, 79)),
        ["onPrimaryContainer"]      = (new(30, 100, 100), new(90, 0, 3)),
        ["inversePrimary"]          = (new(80, 80, 80),  new(40, 31, 31)),
        ["primaryFixed"]            = (new(90, 46, 31),  new(90, 90, 90)),
        ["primaryFixedDim"]         = (new(80, 36, 21),  new(80, 80, 80)),
        ["onPrimaryFixed"]          = (new(10, 100, 100), new(10, 5, 0)),
        ["onPrimaryFixedVariant"]   = (new(30, 98, 95),  new(30, 23, 5)),

        // Secondary family
        ["secondary"]               = (new(40, 23, 18),  new(80, 88, 95)),
        ["onSecondary"]             = (new(100, 100, 100), new(20, 15, 0)),
        ["secondaryContainer"]      = (new(90, 46, 31),  new(30, 60, 79)),
        ["onSecondaryContainer"]    = (new(30, 100, 100), new(90, 0, 3)),
        ["secondaryFixed"]          = (new(90, 46, 31),  new(90, 90, 90)),
        ["secondaryFixedDim"]       = (new(80, 36, 21),  new(80, 80, 80)),
        ["onSecondaryFixed"]        = (new(10, 100, 100), new(10, 5, 0)),
        ["onSecondaryFixedVariant"] = (new(30, 98, 95),  new(30, 23, 5)),

        // Tertiary family
        ["tertiary"]                = (new(40, 23, 18),  new(80, 88, 95)),
        ["onTertiary"]              = (new(100, 100, 100), new(20, 15, 0)),
        ["tertiaryContainer"]       = (new(90, 46, 31),  new(30, 60, 79)),
        ["onTertiaryContainer"]     = (new(30, 100, 100), new(90, 0, 3)),
        ["tertiaryFixed"]           = (new(90, 46, 31),  new(90, 90, 90)),
        ["tertiaryFixedDim"]        = (new(80, 36, 21),  new(80, 80, 80)),
        ["onTertiaryFixed"]         = (new(10, 100, 100), new(10, 5, 0)),
        ["onTertiaryFixedVariant"]  = (new(30, 98, 95),  new(30, 23, 5)),

        // Surface / neutral family
        ["surface"]                 = (new(98, 98, 98),  new(6, 6, 6)),
        ["surfaceVariant"]          = (new(90, 90, 90),  new(30, 30, 30)),
        ["inverseSurface"]          = (new(20, 20, 20),  new(90, 90, 90)),
        ["inverseOnSurface"]        = (new(95, 95, 100), new(20, 17, 0)),
        ["surfaceDim"]              = (new(87, 80, 75),  new(6, 6, 6)),
        ["surfaceBright"]           = (new(98, 98, 98),  new(24, 29, 34)),
        ["surfaceContainerLowest"]  = (new(100, 100, 100), new(4, 2, 0)),
        ["surfaceContainerLow"]     = (new(96, 96, 95),  new(10, 11, 12)),
        ["surfaceContainer"]        = (new(94, 92, 90),  new(12, 16, 20)),
        ["surfaceContainerHigh"]    = (new(92, 88, 85),  new(17, 21, 25)),
        ["surfaceContainerHighest"] = (new(90, 84, 80),  new(22, 26, 30)),
        ["onSurface"]               = (new(10, 5, 0),    new(90, 100, 100)),
        ["onSurfaceVariant"]        = (new(30, 23, 0),   new(80, 88, 100)),
    };

    /// <summary>Standard tones for a role in the given mode, or null if the role is unknown.</summary>
    public static ContrastTones? Get(string role, DayNightMode mode) {
        if (!Map.TryGetValue(role, out var tones)) {
            return null;
        }
        return mode == DayNightMode.Light ? tones.Light : tones.Dark;
    }
}
