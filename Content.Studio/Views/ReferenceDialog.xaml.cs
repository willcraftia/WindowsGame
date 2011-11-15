#region Using

using System.Windows;
using System.Windows.Input;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    /// <summary>
    /// ReferenceDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ReferenceDialog : Window
    {
        public ReferenceDialog()
        {
            InitializeComponent();
        }

        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
