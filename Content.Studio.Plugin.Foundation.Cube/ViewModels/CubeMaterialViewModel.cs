#region Using

using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Content.Studio.Models;
using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Win.Framework;
using Willcraftia.Win.Xna.Framework;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.Cube.ViewModels
{
    using MediaColor = System.Windows.Media.Color;

    public sealed class CubeMaterialViewModel : ContentViewModelBase
    {
        public CubeMaterial CubeMaterial
        {
            get { return Model as CubeMaterial; }
        }

        public EffectReferenceViewModel Effect { get; private set; }

        public CubeTextureViewModel Diffuse { get; private set; }

        public float Alpha
        {
            get { return CubeMaterial.Alpha; }
            set
            {
                if (CubeMaterial.Alpha == value) return;

                CubeMaterial.Alpha = value;
                RaisePropertyChanged("Alpha");
            }
        }

        public MediaColor DiffuseColor
        {
            get { return CubeMaterial.DiffuseColor.ToMediaColor(); }
            set
            {
                if (CubeMaterial.DiffuseColor.ToMediaColor() == value) return;

                CubeMaterial.DiffuseColor = value.ToVector3();
                RaisePropertyChanged("DiffuseColor");

                RefreshThumbnail();
            }
        }

        public MediaColor EmissiveColor
        {
            get { return CubeMaterial.EmissiveColor.ToMediaColor(); }
            set
            {
                if (CubeMaterial.EmissiveColor.ToMediaColor() == value) return;

                CubeMaterial.EmissiveColor = value.ToVector3();
                RaisePropertyChanged("EmissiveColor");
            }
        }

        public MediaColor SpecularColor
        {
            get { return CubeMaterial.SpecularColor.ToMediaColor(); }
            set
            {
                if (CubeMaterial.SpecularColor.ToMediaColor() == value) return;

                CubeMaterial.SpecularColor = value.ToVector3();
                RaisePropertyChanged("SpecularColor");
            }
        }

        public float SpecularPower
        {
            get { return CubeMaterial.SpecularPower; }
            set
            {
                if (CubeMaterial.SpecularPower == value) return;

                CubeMaterial.SpecularPower = value;
                RaisePropertyChanged("SpecularPower");
            }
        }

        public bool VertexColorEnabled
        {
            get { return CubeMaterial.VertexColorEnabled; }
            set
            {
                if (CubeMaterial.VertexColorEnabled == value) return;

                CubeMaterial.VertexColorEnabled = value;
                RaisePropertyChanged("VertexColorEnabled");
            }
        }

        public ImageSource Thumbnail { get; private set; }

        public CubeMaterialViewModel(FileInfo file)
            : base(file)
        {
            Effect = new EffectReferenceViewModel(File, new PropertyModel<ExternalReference<EffectContent>>(Model, "Effect"));

            Diffuse = new CubeTextureViewModel(File, CubeMaterial.Diffuse);

            RefreshThumbnail();
        }

        void RefreshThumbnail()
        {
            if (Diffuse.Texture.Image != null)
            {
                Thumbnail = Diffuse.Texture.Image;
            }
            else
            {
                var drawing = new GeometryDrawing();
                drawing.Geometry = new RectangleGeometry(new Rect(0, 0, 16, 16));
                drawing.Brush = new SolidColorBrush(CubeMaterial.DiffuseColor.ToMediaColor());
                Thumbnail = new DrawingImage(drawing);
            }
        }
    }
}
