#region Using

using Willcraftia.Xna.Foundation.Scenes;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.ViewModels
{
    public class AssetModelActorModelViewModel : ActorModelViewModel
    {
        public string ModelAssetName
        {
            get { return (Model as AssetModelActorModel).ModelAssetName; }
            set
            {
                var assetModel = Model as AssetModelActorModel;

                if (assetModel.ModelAssetName == value) return;

                assetModel.ModelAssetName = value;
                RaisePropertyChanged("ModelAssetName");
            }
        }

        public AssetModelActorModelViewModel(AssetModelActorModel model)
            : base(model)
        {
        }
    }
}
