#region Using

using GalaSoft.MvvmLight.Messaging;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class EditReferenceMessage : GenericMessage<ReferenceEditViewModel>
    {
        public bool? Result { get; set; }

        public EditReferenceMessage(ReferenceEditViewModel content)
            : base(content)
        {
        }
    }
}
