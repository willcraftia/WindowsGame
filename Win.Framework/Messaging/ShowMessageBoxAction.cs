#region Using

using System.Windows;
using GalaSoft.MvvmLight.Messaging;

#endregion

namespace Willcraftia.Win.Framework.Messaging
{
    public sealed class ShowMessageBoxAction : MessageActionBase<DialogMessage>
    {
        protected override void InvokeAction(DialogMessage message)
        {
            var result = MessageBox.Show(
                message.Content, message.Caption, message.Button, message.Icon, message.DefaultResult, message.Options);
            message.ProcessCallback(result);
        }
    }
}
