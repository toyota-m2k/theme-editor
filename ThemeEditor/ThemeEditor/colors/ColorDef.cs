using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThemeEditor.colors;

/**
 * Material 3 のカラーテーマの構成は、次のようになっている。
 * Theme
 *   - LightTheme
 *      - PrimaryTheme
 *          - Normal Contrast
 *          - Medium Contrast
 *          - High Contrast
 *      - SecondaryTheme
 *          - Normal Contrast
 *          - Medium Contrast
 *          - High Contrast
 *      - TertiaryTheme
 *          - Normal Contrast
 *          - Medium Contrast
 *          - High Contrast
 *   - DarkTheme
 *      ...
 *          
 * これに対して、このツールのクラス構成は次のようになっている。
 * 
 * AndroidThemeSet (ルート:theme)
 *   - Light : AndroidContrastColorTheme
 *      - Normal : AndroidColorTheme
 *          - Primary : PrimaryTheme
 *          - Secondary : SecondaryTheme
 *          - Tertiary : TertiaryTheme
 *      - Medium : AndroidColorTheme
 *          - Primary : PrimaryTheme
 *          - Secondary : SecondaryTheme
 *          - Tertiary : TertiaryTheme
 *      - High : AndroidColorTheme
 *          - Primary : PrimaryTheme
 *          - Secondary : SecondaryTheme
 *          - Tertiary : TertiaryTheme
 *   - Dark  : AndroidContrastColorTheme
 *      ...
 */

/**
 * Light/Dark のモードを表す列挙型
 */
public enum DayNightMode {
    Light,
    Dark,
}

/**
 * Normal/Medium/High のコントラストを表す列挙型
 */
public enum ColorContrast {
    Normal,
    Medium,
    High,
}

/**
 * Primary/Secondary/Tertiary のテーマを表す列挙型
 */
public enum CommonThemeType {
    Primary,
    Secondary,
    Tertiary,
}

/**
 * 色の交換対象を表す列挙型
 */
public enum ExchangeColorTarget {
    Primary_Secondary,
    Primary_Tertiary,
    Secondary_Tertiary,
}

