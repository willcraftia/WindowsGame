﻿#region Using

using System.Windows;
using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Win.Framework.Messaging;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    public sealed class ReferPluginAssemblyFileMessageAction : MessageActionBase<ReferPluginAssemblyFileMessage>
    {
        protected override void InvokeAction(ReferPluginAssemblyFileMessage message)
        {
            var d = new PluginAssemblyFileDialog();
            d.DataContext = message.Content;
            d.Owner = AssociatedObject as Window;
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
