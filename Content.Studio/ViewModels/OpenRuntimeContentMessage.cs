#region Using

using GalaSoft.MvvmLight.Messaging;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class OpenRuntimeContentMessage : GenericMessage<RuntimeContentViewModel>
    {
        public OpenRuntimeContentMessage(RuntimeContentViewModel content)
            : base(content)
        {
        }
    }
}
