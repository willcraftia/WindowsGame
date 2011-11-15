#region Using

using System;
using System.Windows;
using System.Windows.Controls;
using Willcraftia.Content.Studio.ViewModels;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Views
{
    /// <summary>
    /// TextureControl.xaml の相互作用ロジック
    /// </summary>
    public partial class TextureControl : UserControl
    {
        public TextureControl()
        {
            InitializeComponent();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            TextureFormControl.RuntimeContent = DataContext as RuntimeContentViewModel;
            TextureFormControl.LoadContent();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            TextureFormControl.UnloadContent();
            TextureFormControl.RuntimeContent = null;
        }
    }
}
