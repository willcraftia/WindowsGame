#region Using

using System.Windows;
using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Win.Framework.Messaging;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    public sealed class CloseWindowMessageAction : MessageActionBase<CloseWindowMessage>
    {
        protected override void InvokeAction(CloseWindowMessage message)
        {
            var window = AssociatedObject as Window;
            if (window.DataContext != message.Sender) return;

            window.Close();
        }
    }
}
