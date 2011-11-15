#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

#endregion

namespace Willcraftia.Win.Xna.Framework.Design
{
    public sealed class XmlReferenceEditor<T> : ExternalReferenceEditor<T>
    {
        protected override void InitializeDialog(Microsoft.Win32.OpenFileDialog dialog)
        {
            base.InitializeDialog(dialog);

            dialog.Filter =
                "XML file (*.xml)|*.xml" +
                "|All files (*.*)|*.*";
            dialog.AddExtension = true;
            dialog.DefaultExt = "*.xml";
        }
    }
}
