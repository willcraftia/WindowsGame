#region Using

using System.IO;
using Willcraftia.Xna.Foundation.Scenes;
using Willcraftia.Content.Studio.Models;
using Willcraftia.Content.Studio.ViewModels;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.ViewModels
{
    public sealed class ActorViewModel : ContentViewModelBase
    {
        public ActorModelViewModel ActorModel { get; private set; }

        public ActorViewModel(FileInfo file)
            : base(file)
        {
            if ((Model as Actor).ActorModel != null)
            {
                ActorModel = ActorModelViewModelFactory.CreateViewModel((Model as Actor).ActorModel);
            }
        }
    }
}
