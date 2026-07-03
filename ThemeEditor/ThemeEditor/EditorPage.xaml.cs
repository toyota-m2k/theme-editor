using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ThemeEditor {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditorPage : Page {
        EditorViewModel ViewModel = new EditorViewModel();
        private string? baseName = null;

        private async Task<StorageFolder?> GetSubFolder(StorageFolder folder, string name) {
            try {
                var subfolder = await folder.GetFolderAsync(name);
                return subfolder;
            }
            catch (Exception e) {
                //Console.WriteLine(e);
                return null;
            }
        }
        private async Task<StorageFile?> GetFile(StorageFolder folder, string name) {
            try {
                var file = await folder.GetFileAsync(name);
                return file;
            }
            catch (Exception e) {
                //Console.WriteLine(e);
                return null;
            }
        }
        private async Task<StorageFile?> GetNightFile(StorageFile lightFile) {
            try {
                var folder = await lightFile.GetParentAsync();
                if (folder.Name == "night") {
                    return null;
                }
                var subFolder = await GetSubFolder(folder, "night");
                if (subFolder != null) {
                    var file = await GetFile(subFolder, lightFile.Name);
                    return file;
                }
                return null;
            }
            catch (Exception e) {
                //Console.WriteLine(e);
                return null;
            }
        }

        public EditorPage() {
            this.InitializeComponent();
            ViewModel.OpenFileCommand.Subscribe(async () => {
                try {
                    var window = ((App)Application.Current).Window;
                    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                    var picker = new Windows.Storage.Pickers.FileOpenPicker();
                    picker.FileTypeFilter.Add(".xml");
                    picker.FileTypeFilter.Add(".zip");
                    WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
                    var file = await picker.PickSingleFileAsync();
                    if (file != null) {
                        //ViewModel.InputFile.Value = file;
                        var builder = AndroidColorThemeSet.Builder.AppendAuto(file);
                        if (file.FileType == ".xml") {
                            var nightFile = await GetNightFile(file);
                            if (nightFile != null) {
                                builder.AppendDark(nightFile);
                            }
                        }
                        ViewModel.ThemeSet.Value = builder.Build();
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            });

            ViewModel.SaveFileCommand.Subscribe(async () => {
                try {
                    baseName = await GetThemeBaseName(baseName);
                    if (baseName == null) return;
                    var window = ((App)Application.Current).Window;
                    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                    var picker = new Windows.Storage.Pickers.FolderPicker();
                    WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
                    var folder = await picker.PickSingleFolderAsync();
                    if (folder != null) {
                        try {
                            var filename = (baseName == "material") ? "colors.xml" : $"colors_{baseName}.xml";
                            var fileLight = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                            var xml = ViewModel.ThemeSet.Value.Light.ToXml(baseName);
                            using (var streamLight = await fileLight.OpenStreamForWriteAsync())
                            using (var writerLight = new StreamWriter(streamLight))
                            {
                                await writerLight.WriteAsync(xml);
                            }

                            folder = await folder.CreateFolderAsync("night", CreationCollisionOption.OpenIfExists);
                            var fileDark = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                            var xmlDark = ViewModel.ThemeSet.Value.Dark.ToXml(baseName);
                            using (var streamDark = await fileDark.OpenStreamForWriteAsync())
                            using (var writerDark = new StreamWriter(streamDark))
                            {
                                await writerDark.WriteAsync(xmlDark);
                            }
                        }
                        catch (Exception e) {
                            await ShowMessageAsync("エラー", e.Message);
                        }
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            });

            ViewModel.CopyThemeCommand.Subscribe(async () => {
                var themeSet = ViewModel.ThemeSet.Value;
                var theme = ViewModel.DayNight.Value == DayNightMode.Light ? themeSet.Light : themeSet.Dark;
                if (theme == null) return;
                baseName = await GetThemeBaseName(baseName);
                if (baseName == null) return;
                var xml = theme.ToXml(baseName);
                if (string.IsNullOrEmpty(xml)) return;

                var dataPackage = new DataPackage() {
                    RequestedOperation = DataPackageOperation.Copy
                };
                dataPackage.SetText(xml);
                try {
                    Clipboard.SetContent(dataPackage);
                    await ShowMessageAsync("Clipboard", $"{ViewModel.DayNight.Value} Colors Copied.");
                    Clipboard.Flush();
                } catch(Exception e) {
                    Debug.WriteLine(e.ToString());
                }
            });

            ViewModel.ExchangeColorCommand.Subscribe(async () => {
                var target = await ExchangeColorDialog();
                if (target == null) return;
                ViewModel.ThemeSet.Value?.ExchangeColors(target.Value);
            });
        }

        public async Task<String?> GetThemeBaseName(String? initial) {
            var inputBox = new TextBox {
                Text = initial ?? "material",
                PlaceholderText = "入力してください",
                MinWidth = 280
            };

            var dialog = new ContentDialog {
                Title = "テーマ名",
                Content = inputBox,
                PrimaryButtonText = "OK",
                CloseButtonText = "キャンセル",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot // WinUI 3では必須
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary) {
                var text = inputBox.Text;
                return text;
            }
            else {
                return null;
            }
        }

        public async Task<ExchangeColorTarget?> ExchangeColorDialog() {
            var radioButtons = new RadioButtons {
                SelectedIndex = 0,
            };
            radioButtons.Items.Add("Primary <-> Secondary");
            radioButtons.Items.Add("Primary <-> Tertiary");
            radioButtons.Items.Add("Secondary <-> Tertiary");

            var dialog = new ContentDialog {
                Title = "入れ替える色の選択",
                Content = radioButtons,
                PrimaryButtonText = "OK",
                CloseButtonText = "キャンセル",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot // WinUI 3では必須
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary) {
                return radioButtons.SelectedIndex switch {
                    0 => ExchangeColorTarget.Primary_Secondary,
                    1 => ExchangeColorTarget.Primary_Tertiary,
                    2 => ExchangeColorTarget.Secondary_Tertiary,
                    _ => null,
                };
            }
            else {
                return null;
            }
        }

        private async Task ShowMessageAsync(string title, string message) {
            var dialog = new ContentDialog {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }
    }
}
