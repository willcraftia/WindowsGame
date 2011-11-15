#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

#endregion

namespace Willcraftia.Win.Xna.Framework.Design
{
    public sealed class ModelReferenceEditor : ExternalReferenceEditor<ModelContent>
    {
        protected override void InitializeDialog(Microsoft.Win32.OpenFileDialog dialog)
        {
            base.InitializeDialog(dialog);

            dialog.Filter =
                "FBX file (*.fbx)|*.fbx" +
                "X file (*.x)|*.x" +
                "|All files (*.*)|*.*";
            dialog.AddExtension = true;
            dialog.DefaultExt = "*.fbx";
        }
    }
}
