#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Win.Xna.Framework;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowSettings settings = new MainWindowSettings();

        WorkspaceViewModel workspace;

        GraphicsDeviceService graphicsDeviceService;

        public MainWindow()
        {
            workspace = new WorkspaceViewModel();
            DataContext = workspace;

            InitializeComponent();

            // ウィンドウ状態を復元します。
            if (0 <= settings.WindowLeft && !double.IsInfinity(settings.WindowLeft)) Left = settings.WindowLeft;
            if (0 <= settings.WindowTop && !double.IsInfinity(settings.WindowTop)) Top = settings.WindowTop;
            if (!double.IsInfinity(settings.WindowWidth)) Width = settings.WindowWidth;
            if (!double.IsInfinity(settings.WindowHeight)) Height = settings.WindowHeight;
            if (settings.WindowMaximized) WindowState = WindowState.Maximized;

            if (!string.IsNullOrEmpty(settings.ProjectFilePath) && File.Exists(settings.ProjectFilePath))
            {
                workspace.OpenProject(settings.ProjectFilePath);
            }
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (AppTraceListener.Current != null)
            {
                AppTraceListener.Current.DelegateDraw += ConsoleWindow.Console.Write;
                AppTraceListener.Current.TraceCacheEnabled = false;
                ConsoleWindow.Console.SetInitialMessage(AppTraceListener.Current.ConsumeTraceCache());
            }

            var source = HwndSource.FromVisual(this) as HwndSource;
            graphicsDeviceService = GraphicsDeviceService.AddRef(GraphicsProfile.HiDef, source.Handle, 0, 0);
            workspace.Services.AddService<IGraphicsDeviceService>(graphicsDeviceService);
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // 現在のプロジェクト ファイル パスを記憶させます。
            settings.ProjectFilePath = workspace.ProjectFilePath;

            // 必要ならば変更を保存してプロジェクトを閉じます。
            workspace.CloseProjectAfterSaveIfNeeded();

            base.OnClosing(e);
        }

        void OnClosed(object sender, EventArgs e)
        {
            if (graphicsDeviceService != null)
            {
                graphicsDeviceService.Release(true);
                graphicsDeviceService = null;
            }

            // ウィンドウの状態を設定として保存します。
            settings.WindowLeft = Left;
            settings.WindowTop = Top;
            settings.WindowWidth = Width;
            settings.WindowHeight = Height;
            settings.WindowMaximized = (WindowState == WindowState.Maximized);
            settings.Save();
        }

        void ExplorerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ExplorerMenuItem.IsChecked)
            {
                ExplorerWindow.Show();
            }
            else
            {
                ExplorerWindow.Hide();
            }
        }

        void ConsoleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ConsoleMenuItem.IsChecked)
            {
                ConsoleWindow.Show();
            }
            else
            {
                ConsoleWindow.Hide();
            }
        }
    }
}
