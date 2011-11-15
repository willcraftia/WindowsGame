#region Using

using System.Windows;
using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Win.Framework.Messaging;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    public sealed class EditReferenceMessageAction : MessageActionBase<EditReferenceMessage>
    {
        protected override void InvokeAction(EditReferenceMessage message)
        {
            var d = new ReferenceDialog();
            d.DataContext = message.Content;
            d.Owner = Window.GetWindow(AssociatedObject);
            d.Loaded += new RoutedEventHandler(OnDialogLoaded);

            message.Result = d.ShowDialog();
        }

        void OnDialogLoaded(object sender, RoutedEventArgs e)
        {
            var window = sender as Window;
            WindowHelper.PositionToCenter(window, AssociatedObject);
        }
    }
}
