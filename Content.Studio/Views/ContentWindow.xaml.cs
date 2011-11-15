#region Using

using System.Windows;
using Willcraftia.Content.Studio.ViewModels;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    /// <summary>
    /// FileContentWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ContentWindow : Window
    {
        public ContentWindow()
        {
            InitializeComponent();
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ContentGrid.Children.Clear();

            var content = DataContext as ContentViewModelBase;
            if (content == null) return;

            // コンテンツのプロパティを操作するためのコントロールを生成します。
            var control = ContentControlFactory.Instance.CreateControl(content.GetType());
            if (control == null) return;

            // コントロールを追加します。
            ContentGrid.Children.Add(control);
        }
    }
}
