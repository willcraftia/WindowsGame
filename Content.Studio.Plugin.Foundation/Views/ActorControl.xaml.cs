#region Using

using System;
using System.Windows;
using System.Windows.Controls;
using Willcraftia.Content.Studio.Plugin.Foundation.ViewModels;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.Views
{
    /// <summary>
    /// ActorControl.xaml の相互作用ロジック
    /// </summary>
    public partial class ActorControl : UserControl
    {
        public ActorControl()
        {
            InitializeComponent();
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ActorModelGrid.Children.Clear();

            var actor = DataContext as ActorViewModel;
            if (actor == null || actor.ActorModel == null) return;

            var control = ActorModelControlFactory.CreateControl(actor.ActorModel.GetType());
            if (control != null)
            {
                // コントロールを追加します。
                ActorModelGrid.Children.Add(control);
            }
        }
    }
}
