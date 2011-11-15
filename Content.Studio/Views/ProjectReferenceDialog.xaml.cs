#region Using

using System.Windows;
using System.Windows.Input;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    /// <summary>
    /// ProjectReferenceDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ProjectReferenceDialog : Window
    {
        public ProjectReferenceDialog()
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
