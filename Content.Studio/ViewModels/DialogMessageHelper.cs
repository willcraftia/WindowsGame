#region Using

using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public static class DialogMessageHelper
    {
        public static DialogMessage CreateErrorDialogBoxMessage(string content, Action<MessageBoxResult> callback)
        {
            var message = new DialogMessage(content, callback);
            message.Button = MessageBoxButton.OK;
            message.Icon = MessageBoxImage.Error;
            message.DefaultResult = MessageBoxResult.OK;
            message.Options = MessageBoxOptions.None;
            return message;
        }

        public static DialogMessage CreateConfirmDialogBoxMessage(string content, Action<MessageBoxResult> callback)
        {
            var message = new DialogMessage(content, callback);
            message.Button = MessageBoxButton.YesNo;
            message.Icon = MessageBoxImage.Question;
            message.DefaultResult = MessageBoxResult.No;
            message.Options = MessageBoxOptions.None;
            return message;
        }
    }
}
