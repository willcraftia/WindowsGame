#region Using

using System;
using System.Windows;
using System.Windows.Controls;
using Willcraftia.Content.Studio.ViewModels;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Views
{
    /// <summary>
    /// SpriteFontControl.xaml の相互作用ロジック
    /// </summary>
    public partial class SpriteFontControl : UserControl
    {
        public SpriteFontControl()
        {
            InitializeComponent();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            SpriteFontFormControl.RuntimeContent = DataContext as RuntimeContentViewModel;
            SpriteFontFormControl.LoadContent();
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            SpriteFontFormControl.UnloadContent();
            SpriteFontFormControl.RuntimeContent = null;
        }
    }
}
