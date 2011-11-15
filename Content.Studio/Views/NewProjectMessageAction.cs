#region Using

using System.Windows;
using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Win.Framework.Messaging;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    public sealed class NewProjectMessageAction : MessageActionBase<NewProjectMessage>
    {
        protected override void InvokeAction(NewProjectMessage message)
        {
            var d = new NewProjectDialog();
            d.Owner = AssociatedObject as Window;
            d.Loaded += new RoutedEventHandler(OnDialogLoaded);

            message.Result = d.ShowDialog();
            message.ProjectName = d.ProjectName;
            message.DirectoryPath = d.DirectoryPath;
        }

        void OnDialogLoaded(object sender, RoutedEventArgs e)
        {
            var window = sender as Window;
            WindowHelper.PositionToCenter(window, AssociatedObject);
        }
    }
}
