#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

#endregion

namespace Willcraftia.Win.Xna.Framework.Design
{
    public sealed class TextureReferenceEditor : ExternalReferenceEditor<TextureContent>
    {
        protected override void InitializeDialog(Microsoft.Win32.OpenFileDialog dialog)
        {
            base.InitializeDialog(dialog);

            dialog.Filter =
                "PNG file (*.png)|*.png" +
                "|BMP file (*.bmp)|*.bmp" +
                "|JPEG file (*.jpg;*.jpeg)|*.jpg;*.jpeg" +
                "|DDS file (*.dds)|*.dds" +
                "|TGA file (*.tga)|*.tga" +
                "|Other image files (*.dib;*.hdr;*.pfm;*.ppm)|*.dib;*.hdr;*.pfm;*.ppm" +
                "|All files (*.*)|*.*";
            dialog.AddExtension = true;
            dialog.DefaultExt = "*.png";
        }
    }
}
