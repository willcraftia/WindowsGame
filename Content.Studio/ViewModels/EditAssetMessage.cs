#region Using

using GalaSoft.MvvmLight.Messaging;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class EditAssetMessage : GenericMessage<AssetEditViewModel>
    {
        public bool? Result { get; set; }

        public EditAssetMessage(AssetEditViewModel content)
            : base(content)
        {
        }
    }
}
