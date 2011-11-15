#region Using

using System.Windows;
using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Win.Framework.Messaging;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    public sealed class OpenRuntimeContentMessageAction : MessageActionBase<OpenRuntimeContentMessage>
    {
        protected override void InvokeAction(OpenRuntimeContentMessage message)
        {
            var d = new RuntimeContentWindow();
            d.DataContext = message.Content;
            d.Owner = AssociatedObject as Window;
            d.Loaded += new RoutedEventHandler(OnDialogLoaded);
            d.Show();
        }

        void OnDialogLoaded(object sender, RoutedEventArgs e)
        {
            var window = sender as Window;
            WindowHelper.PositionToCenter(window, AssociatedObject);
        }
    }
}
