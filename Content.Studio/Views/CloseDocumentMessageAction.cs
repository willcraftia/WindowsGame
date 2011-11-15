#region Using

using System.Windows;
using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Win.Framework.Messaging;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    public sealed class CloseDocumentMessageAction : MessageActionBase<CloseDocumentMessage>
    {
        protected override void InvokeAction(CloseDocumentMessage message)
        {
            var document = AssociatedObject as ContentDocument;
            if (document.DataContext == message.Sender)
            {
                document.Close();
            }
        }
    }
}
