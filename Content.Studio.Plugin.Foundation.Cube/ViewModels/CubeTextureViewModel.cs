#region Using

using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Win.Framework;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.Cube.ViewModels
{
    public sealed class CubeTextureViewModel : ViewModelBase
    {
        readonly FileInfo ownerFile;
        readonly CubeTexture model;

        public TextureContentReferenceViewModel Texture { get; private set; }

        public float Scale
        {
            get { return model.Scale; }
            set
            {
                if (model.Scale == value) return;

                model.Scale = value;
                RaisePropertyChanged("Scale");
            }
        }

        public bool GenerateMipmaps
        {
            get { return model.GenerateMipmaps; }
            set
            {
                if (model.GenerateMipmaps == value) return;

                model.GenerateMipmaps = value;
                RaisePropertyChanged("GenerateMipmaps");
            }
        }

        public TextureProcessorOutputFormat TextureFormat
        {
            get { return model.TextureFormat; }
            set
            {
                if (model.TextureFormat == value) return;

                model.TextureFormat = value;
                RaisePropertyChanged("TextureFormat");
            }
        }

        public CubeTextureViewModel(FileInfo ownerFile, CubeTexture model)
        {
            this.ownerFile = ownerFile;
            this.model = model;

            Texture = new TextureContentReferenceViewModel(ownerFile,
                new PropertyModel<ExternalReference<TextureContent>>(model, "Texture"));
        }
    }
}
