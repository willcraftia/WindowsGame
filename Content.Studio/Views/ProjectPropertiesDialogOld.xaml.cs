#region Using

using System.Windows;
using System.Windows.Input;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    /// <summary>
    /// ProjectPropertiesDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ProjectPropertiesDialogOld : Window
    {
        public ProjectPropertiesDialogOld()
        {
            InitializeComponent();
        }

        void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
