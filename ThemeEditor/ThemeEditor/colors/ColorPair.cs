using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ThemeEditor.parser;

namespace ThemeEditor.colors;

/**
 * 背景色と前景色のペア
 * ViewModel で使うことを想定しているので、ReactiveProperty を使っている。
 */
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


