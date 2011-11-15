#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

#endregion

namespace Willcraftia.Win.Xna.Framework.Design
{
    public sealed class EffectReferenceEditor : ExternalReferenceEditor<EffectContent>
    {
        protected override void InitializeDialog(Microsoft.Win32.OpenFileDialog dialog)
        {
            base.InitializeDialog(dialog);

            dialog.Filter =
                "FX file (*.fx)|*.fx" +
                "|All files (*.*)|*.*";
            dialog.AddExtension = true;
            dialog.DefaultExt = "*.fx";
        }
    }
}
