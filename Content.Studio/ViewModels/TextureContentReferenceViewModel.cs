#region Using

using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Win.Framework;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class TextureContentReferenceViewModel : ExternalReferenceViewModel<TextureContent>
    {
        ImageSource image;
        public ImageSource Image
        {
            get { return image; }
            private set
            {
                if (image == value) return;

                image = value;
                RaisePropertyChanged("Image");
            }
        }

        public TextureContentReferenceViewModel(FileInfo ownerFile, PropertyModel<ExternalReference<TextureContent>> propertyModel)
            : base(ownerFile, propertyModel)
        {
            RefreshImage();
        }

        protected override void OnModelChanged()
        {
            RefreshImage();

            base.OnModelChanged();
        }

        void RefreshImage()
        {
            if (Model == null || string.IsNullOrEmpty(Model.Filename))
            {
                Image = null;
            }
            else
            {
                Image = new BitmapImage(new Uri(Model.Filename));
            }
        }
    }
}
