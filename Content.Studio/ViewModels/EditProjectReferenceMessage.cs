#region Using

using GalaSoft.MvvmLight.Messaging;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class EditProjectReferenceMessage : GenericMessage<ProjectReferenceEditViewModel>
    {
        public bool? Result { get; set; }

        public EditProjectReferenceMessage(ProjectReferenceEditViewModel content)
            : base(content)
        {
        }
    }
}
