using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThemeEditor.colors;

public interface ICommonTheme {
    ColorPair Base { get; }
    ColorPair Container { get; }
    ColorPair Fixed { get; }
    ColorPair FixedDim { get; }
}

/**
 * primaryInverse という名前から、Primary のカラーセットに含めておくが、
 * これは、inverseSurface上に表示する前景色（inverseOnSurface のアクセント色）である。
 */
public interface IPrimaryTheme : ICommonTheme {
    ColorPair Inverse { get; }
}
