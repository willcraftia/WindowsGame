#region Using

using System.Windows;
using Willcraftia.Content.Studio.ViewModels;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    /// <summary>
    /// RuntimeContentWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class RuntimeContentWindow : Window
    {
        public RuntimeContentWindow()
        {
            InitializeComponent();
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var content = DataContext as RuntimeContentViewModel;
            if (content == null || content.ContentType == null) return;

            var control = RuntimeContentControlFactory.Instance.CreateControl(content.ContentType);
            var frameworkElement = control as FrameworkElement;
            if (frameworkElement != null)
            {
                RuntimeContentView.Children.Add(frameworkElement);
            }
        }
    }
}
