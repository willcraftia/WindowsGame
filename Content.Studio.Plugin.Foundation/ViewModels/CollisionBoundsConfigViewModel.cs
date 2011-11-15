#region Using

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Xna.Foundation.Scenes;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.ViewModels
{
    public sealed class CollisionBoundsConfigViewModel : ViewModelBase
    {
        readonly CollisionBoundsConfig model;

        public CollisionBoundsConfigViewModel(CollisionBoundsConfig model)
            : base(Messenger.Default)
        {
            this.model = model;
        }
    }
}
