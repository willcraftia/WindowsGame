#region Using

using GalaSoft.MvvmLight.Messaging;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class ReferPluginAssemblyFileMessage : GenericMessage<PluginAssemblyFileViewModel>
    {
        public bool? Result { get; set; }

        public ReferPluginAssemblyFileMessage(PluginAssemblyFileViewModel content)
            : base(content)
        {
        }
    }
}
