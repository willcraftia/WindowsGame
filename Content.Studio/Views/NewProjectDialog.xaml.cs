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
using Willcraftia.Win.Framework;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    /// <summary>
    /// NewProjectDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class NewProjectDialog : Window
    {
        sealed class ViewModel : ObservableObject
        {
            public string projectName;
            public string ProjectName
            {
                get { return projectName; }
                set
                {
                    if (projectName == value) return;

                    projectName = value;
                    OnPropertyChanged("ProjectName");
                }
            }

            string directoryPath;
            public string DirectoryPath
            {
                get { return directoryPath; }
                set
                {
                    if (directoryPath == value) return;

                    directoryPath = value;
                    OnPropertyChanged("DirectoryPath");
                }
            }
        }

        public string ProjectName
        {
            get { return (DataContext as ViewModel).ProjectName; }
        }

        public string DirectoryPath
        {
            get { return (DataContext as ViewModel).DirectoryPath; }
        }

        public NewProjectDialog()
        {
            InitializeComponent();
            DataContext = new ViewModel();
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        void SelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            var d = new System.Windows.Forms.FolderBrowserDialog();
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                (DataContext as ViewModel).DirectoryPath = d.SelectedPath;
            }
        }
    }
}
