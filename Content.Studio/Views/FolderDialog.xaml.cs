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
using System.Windows.Shapes;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    /// <summary>
    /// FolderWidow.xaml の相互作用ロジック
    /// </summary>
    public partial class FolderDialog : Window
    {
        public string FolderName
        {
            get { return NameTextBox.Text; }
            set { NameTextBox.Text = value; }
        }

        public FolderDialog()
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
