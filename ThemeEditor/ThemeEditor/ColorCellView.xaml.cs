using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using static ThemeEditor.AndroidColorTheme;
using Reactive.Bindings;
using System.Reactive.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ThemeEditor
{
    public sealed partial class ColorCellView : UserControl
    {
        public ReactiveProperty<ColorPair> ColorPair { get; } = new();
        public ColorPair Colors {
            get => ColorPair.Value;
            set => ColorPair.Value = value;
        }
        public ReactiveCommand CommandBg { get; } = new();
        public ReactiveCommand CommandFg { get; } = new();
        public ReactiveCommand ColorChanged { get; } = new();
        private bool changingBackground = false;

        public ColorCellView()
        {
            this.InitializeComponent();
            CommandBg.Subscribe(async () => {
                changingBackground = true;
                colorPicker.Color = Colors.Background.Value.AsUiColor;
                await colorPickerDialog.ShowAsync();
            });
            CommandFg.Subscribe(async () => {
                changingBackground = false;
                colorPicker.Color = Colors.Foreground.Value.AsUiColor;
                await colorPickerDialog.ShowAsync();
            });
            ColorChanged.Subscribe(() => {
                if (changingBackground) {
                    Colors.Background.Value = new NamedColor(Colors.Background.Value.Name, colorPicker.Color.ToString());
                    EditorViewModel.BackgroundColorChanged.Execute(Colors.Background.Value);
                }
                else {
                    Colors.Foreground.Value = new NamedColor(Colors.Foreground.Value.Name, colorPicker.Color.ToString());
                }
            });
        }
    }
}
