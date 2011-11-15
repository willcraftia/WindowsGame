#region Using

using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Win.Framework.Messaging;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    public sealed class ReferFileMessageAction : MessageActionBase<ReferFileMessage>
    {
        protected override void InvokeAction(ReferFileMessage message)
        {
            var d = new Microsoft.Win32.OpenFileDialog();
            d.AddExtension = message.AddExtension;
            d.CheckFileExists = message.CheckFileExists;
            d.DefaultExt = message.DefaultExt;
            d.DereferenceLinks = message.DereferenceLinks;
            d.FileName = message.FileName;
            d.Filter = message.Filter;
            d.FilterIndex = message.FilterIndex;
            d.InitialDirectory = message.InitialDirectory;
            d.Title = message.Title;
            d.ValidateNames = message.ValidateNames;
            d.Multiselect = message.Multiselect;
            d.ReadOnlyChecked = message.ReadOnlyChecked;
            d.ShowReadOnly = message.ShowReadOnly;

            message.Result = d.ShowDialog();
            message.FileName = d.FileName;
        }
    }
}
