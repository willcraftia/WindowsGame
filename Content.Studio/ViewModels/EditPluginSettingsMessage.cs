#region Using

using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Xna.Framework.Content.Build;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class EditPluginSettingsMessage : GenericMessage<PluginSettingsEditViewModel>
    {
        public bool? Result { get; set; }

        public EditPluginSettingsMessage(PluginSettingsEditViewModel content)
            : base(content)
        {
        }
    }
}
