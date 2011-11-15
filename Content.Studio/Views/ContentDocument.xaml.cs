#region Using

using System.Windows;
using Willcraftia.Content.Studio.ViewModels;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    /// <summary>
    /// ContentDocument.xaml の相互作用ロジック
    /// </summary>
    public partial class ContentDocument : AvalonDock.DocumentContent
    {
        public ContentDocument()
        {
            InitializeComponent();
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ContentGrid.Children.Clear();

            if (DataContext == null) return;

            // コンテンツのプロパティを操作するためのコントロールを生成します。
            var control = ContentControlFactory.Instance.CreateControl(DataContext.GetType());
            if (control == null) return;

            // コントロールを追加します。
            ContentGrid.Children.Add(control);
        }
    }
}
