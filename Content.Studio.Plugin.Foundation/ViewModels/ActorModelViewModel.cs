#region Using

using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Xna.Foundation.Scenes;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.ViewModels
{
    public class ActorModelViewModel : ViewModelBase
    {
        protected ActorModel Model { get; private set; }

        public Type ModelType
        {
            get { return Model.GetType(); }
        }

        public float MaxDrawDistance
        {
            get { return Model.MaxDrawDistance; }
            set
            {
                if (Model.MaxDrawDistance == value) return;

                Model.MaxDrawDistance = value;
                RaisePropertyChanged("MaxDrawDistance");
            }
        }

        public bool NearTransparencyEnabled
        {
            get { return Model.NearTransparencyEnabled; }
            set
            {
                if (NearTransparencyEnabled == value) return;

                NearTransparencyEnabled = value;
                RaisePropertyChanged("NearTransparencyEnabled");
            }
        }

        public bool CullingTransparencyEnabled
        {
            get { return Model.CullingTransparencyEnabled; }
            set
            {
                if (CullingTransparencyEnabled == value) return;

                CullingTransparencyEnabled = value;
                RaisePropertyChanged("CullingTransparencyEnabled");
            }
        }

        public bool CastShadowEnabled
        {
            get { return Model.CastShadowEnabled; }
            set
            {
                if (CastShadowEnabled == value) return;

                CastShadowEnabled = value;
                RaisePropertyChanged("CastShadowEnabled");
            }
        }

        public ActorModelViewModel(ActorModel model)
            : base(Messenger.Default)
        {
            Model = model;
        }
    }
}
