#region Using

using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Xna.Framework.Content.Build;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class EditProjectPropertiesMessage : GenericMessage<ProjectPropertiesEditViewModel>
    {
        public bool? Result { get; set; }

        public EditProjectPropertiesMessage(ProjectPropertiesEditViewModel content)
            : base(content)
        {
        }
    }
}
