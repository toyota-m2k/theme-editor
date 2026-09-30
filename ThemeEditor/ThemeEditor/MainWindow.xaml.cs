using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using ThemeEditor.common;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ThemeEditor
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            string AppName;
            Version? AppVersion = null;
            this.InitializeComponent();
            if (RuntimeHelper.IsMSIX) {
                var package = Windows.ApplicationModel.Package.Current;
                AppName = package.DisplayName;
                var v = package.Id.Version;
                AppVersion = new(v.Major, v.Minor, v.Build, v.Revision);
            }
            else {
                AppName = "SVG Path Editor (u)";
            }
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();
            var assemblyVersion = assemblyName.Version ?? new Version(0, 0, 0, 0);
            if (AppVersion == null) {
                AppVersion = assemblyVersion;
            }
            else {
                // プロジェクトプロパティ(csproj)の「アセンブリバージョン」と、Package.manifestの <Identity ... Version> の値は、
                // 常に同じ値に設定しておくこと。
                Debug.Assert(AppVersion == assemblyVersion);
            }

            var title = $"{AppName} v{AppVersion}";

#if DEBUG
            title += " <debug>";
#endif
            this.Title = title;
            this.AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/program-icon.ico"));
        }

    }
}
