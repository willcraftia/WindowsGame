#region Using

using GalaSoft.MvvmLight.Messaging;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class EditContentMessage : GenericMessage<ContentViewModelBase>
    {
        public EditContentMessage(ContentViewModelBase content)
            : base(content)
        {
        }
    }
}
