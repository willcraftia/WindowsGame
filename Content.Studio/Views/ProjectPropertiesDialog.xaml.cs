#region Using

using System.Windows;
using System.Windows.Input;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    /// <summary>
    /// ProjectPropertiesDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ProjectPropertiesDialog : Window
    {
        public ProjectPropertiesDialog()
        {
            InitializeComponent();
        }

        void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
