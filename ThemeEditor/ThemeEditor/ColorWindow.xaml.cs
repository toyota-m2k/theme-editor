using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace ThemeEditor {
    public sealed partial class ColorWindow : Window {
        internal EditorViewModel ViewModel;

        internal ColorWindow(EditorViewModel viewModel) {
            ViewModel = viewModel;
            this.InitializeComponent();
        }

        private CommonThemeType SelectedType {
            get {
                var idx = TypeSelector.SelectedIndex;
                if (idx < 0) idx = 0;
                return (CommonThemeType)idx;
            }
        }

        private void OnStockClick(object sender, RoutedEventArgs e) {
            var themeSet = ViewModel.ThemeSet.Value;
            if (themeSet == null) return;
            var type = SelectedType;
            var title = $"{type} {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            ViewModel.Stocker.Add(themeSet, type, title);
        }

        private void OnApplyClick(object sender, RoutedEventArgs e) {
            if (StockList.SelectedItem is not StockItem item) return;
            var themeSet = ViewModel.ThemeSet.Value;
            if (themeSet == null) return;
            item.ApplyTo(themeSet, SelectedType);
        }

        private void OnRemoveClick(object sender, RoutedEventArgs e) {
            if (StockList.SelectedItem is not StockItem item) return;
            ViewModel.Stocker.Remove(item);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            var item = StockList.SelectedItem as StockItem;
            var hasSelection = item != null;
            ApplyButton.IsEnabled = hasSelection;
            RemoveButton.IsEnabled = hasSelection;
            BuildPreview(item);
        }

        private void BuildPreview(StockItem? item) {
            PreviewGrid.Children.Clear();
            PreviewGrid.ColumnDefinitions.Clear();
            PreviewGrid.RowDefinitions.Clear();

            if (item == null) {
                PreviewTitle.Text = "";
                return;
            }

            PreviewTitle.Text = $"{item.Title}  ({item.Type})";

            var columns = new (string Header, StockedTheme Theme)[] {
                ("Light / Normal", item.ThemeSet.Light.Normal),
                ("Light / Medium", item.ThemeSet.Light.Medium),
                ("Light / High", item.ThemeSet.Light.High),
                ("Dark / Normal", item.ThemeSet.Dark.Normal),
                ("Dark / Medium", item.ThemeSet.Dark.Medium),
                ("Dark / High", item.ThemeSet.Dark.High),
            };
            var rowLabels = new[] { "Base", "Container", "Fixed", "FixedDim", "Inverse" };

            foreach (var _ in columns) {
                PreviewGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(180) });
            }
            PreviewGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            foreach (var _ in rowLabels) {
                PreviewGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(48) });
            }

            for (int c = 0; c < columns.Length; c++) {
                var header = new TextBlock {
                    Text = columns[c].Header,
                    FontSize = 12,
                    Margin = new Thickness(4)
                };
                Grid.SetColumn(header, c);
                Grid.SetRow(header, 0);
                PreviewGrid.Children.Add(header);

                var theme = columns[c].Theme;
                var pairs = new StockedColorPair?[] {
                    theme.Base, theme.Container, theme.Fixed, theme.FixedDim, theme.Inverse
                };
                for (int r = 0; r < pairs.Length; r++) {
                    var pair = pairs[r];
                    if (pair == null) continue;
                    var cell = new StockedColorCellView {
                        Pair = pair,
                        Label = rowLabels[r],
                        Margin = new Thickness(2)
                    };
                    Grid.SetColumn(cell, c);
                    Grid.SetRow(cell, r + 1);
                    PreviewGrid.Children.Add(cell);
                }
            }
        }
    }
}
