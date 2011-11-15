#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    /// <summary>
    /// PropertyGridPreBuildContentControl.xaml の相互作用ロジック
    /// </summary>
    public partial class PropertyGridPreBuildContentControl : UserControl
    {
        public PropertyGridPreBuildContentControl()
        {
            InitializeComponent();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //var file = DataContext as FileInfoViewModel;

            PropertyGrid.SelectedObject = DataContext;
            //PropertyGrid.SelectedObject = file.Asset.PreBuildContent;
            PropertyGrid.ExpandAllGridItems();
        }
    }
}
