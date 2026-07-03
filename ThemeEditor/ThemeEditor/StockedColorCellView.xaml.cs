using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Drawing;

namespace ThemeEditor {
    public sealed partial class StockedColorCellView : UserControl {
        public static readonly DependencyProperty PairProperty =
            DependencyProperty.Register(
                nameof(Pair),
                typeof(StockedColorPair),
                typeof(StockedColorCellView),
                new PropertyMetadata(null, OnPairChanged));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(
                nameof(Label),
                typeof(string),
                typeof(StockedColorCellView),
                new PropertyMetadata("", OnPairChanged));

        public StockedColorPair? Pair {
            get => (StockedColorPair?)GetValue(PairProperty);
            set => SetValue(PairProperty, value);
        }

        public string Label {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public StockedColorCellView() {
            this.InitializeComponent();
        }

        private static void OnPairChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((StockedColorCellView)d).UpdateColors();
        }

        private static Windows.UI.Color ToUiColor(Color color) {
            if (color.IsEmpty) {
                return Windows.UI.Color.FromArgb(0, 0, 0, 0);
            }
            return Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        private static string ToHex(Color color) {
            return color.IsEmpty ? "" : $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        private void UpdateColors() {
            var pair = Pair;
            if (pair == null) {
                RootGrid.Background = null;
                BackgroundText.Text = "";
                ForegroundText.Text = "";
                return;
            }

            var backgroundBrush = new SolidColorBrush(ToUiColor(pair.Background));
            var foregroundBrush = new SolidColorBrush(ToUiColor(pair.Foreground));

            RootGrid.Background = backgroundBrush;

            BackgroundText.Foreground = foregroundBrush;
            ForegroundText.Foreground = foregroundBrush;

            var label = string.IsNullOrEmpty(Label) ? "" : $"{Label}  ";
            BackgroundText.Text = $"{label}{ToHex(pair.Background)}";
            ForegroundText.Text = ToHex(pair.Foreground);
        }
    }
}
