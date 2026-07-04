using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Drawing;

namespace ThemeEditor {
    public sealed partial class StockedThemeSampleView : UserControl {
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register(
                nameof(Theme),
                typeof(StockedTheme),
                typeof(StockedThemeSampleView),
                new PropertyMetadata(null, OnThemeChanged));

        public StockedTheme? Theme {
            get => (StockedTheme?)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        public StockedThemeSampleView() {
            this.InitializeComponent();
        }

        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((StockedThemeSampleView)d).UpdateColors();
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
            var theme = Theme;
            if (theme == null) {
                ContainerGrid.Background = null;
                BaseButton.Background = null;
                CaptionText.Text = "";
                return;
            }

            ContainerGrid.Background = new SolidColorBrush(ToUiColor(theme.Container.Background));
            BaseButton.Background = new SolidColorBrush(ToUiColor(theme.Base.Background));
            CaptionText.Foreground = new SolidColorBrush(ToUiColor(theme.Base.Foreground));
            CaptionText.Text = ToHex(theme.Base.Background);
        }
    }
}
