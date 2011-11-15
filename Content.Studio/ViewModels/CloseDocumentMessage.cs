#region Using

using GalaSoft.MvvmLight.Messaging;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class CloseDocumentMessage : MessageBase
    {
        public CloseDocumentMessage(object sender)
            : base(sender)
        {
        }
    }
}
