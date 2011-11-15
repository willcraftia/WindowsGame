#region Using

using System.Windows;
using System.Windows.Input;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    /// <summary>
    /// PluginAssemblyFileDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class PluginAssemblyFileDialog : Window
    {
        public PluginAssemblyFileDialog()
        {
            InitializeComponent();
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
