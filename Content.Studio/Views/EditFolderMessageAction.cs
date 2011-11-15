#region Using

using System.Windows;
using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Win.Framework.Messaging;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    public sealed class EditFolderMessageAction : MessageActionBase<EditFolderMessage>
    {
        protected override void InvokeAction(EditFolderMessage message)
        {
            var d = new FolderDialog();
            d.Owner = AssociatedObject as Window;
            d.Loaded += new RoutedEventHandler(OnDialogLoaded);
            d.Title = message.Title;

            message.Result = d.ShowDialog();
            message.FolderName = d.FolderName;
        }

        void OnDialogLoaded(object sender, RoutedEventArgs e)
        {
            var window = sender as Window;
            WindowHelper.PositionToCenter(window, AssociatedObject);
        }
    }
}
