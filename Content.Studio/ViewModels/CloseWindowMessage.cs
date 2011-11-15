#region Using

using GalaSoft.MvvmLight.Messaging;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class CloseWindowMessage : MessageBase
    {
        public CloseWindowMessage(object sender)
            : base(sender)
        {
        }
    }
}
