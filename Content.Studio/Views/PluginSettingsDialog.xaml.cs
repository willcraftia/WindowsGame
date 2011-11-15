#region Using

using System.Windows;
using System.Windows.Input;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    /// <summary>
    /// PluginSettingsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PluginSettingsDialog : Window
    {
        public PluginSettingsDialog(Willcraftia.Content.Studio.ViewModels.PluginSettingsEditViewModel model)
        {
            InitializeComponent();
        }

        void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
