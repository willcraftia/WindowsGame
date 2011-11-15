#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;

#endregion

namespace Willcraftia.Win.Xna.Framework.Design
{
    public sealed class XmlFileEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            InitializeDialog(dialog);

            var filename = value as string;

            if (filename != null)
            {
                dialog.FileName = filename;
            }

            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        void InitializeDialog(Microsoft.Win32.OpenFileDialog dialog)
        {
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;

            dialog.Filter =
                "XML file (*.xml)|*.xml" +
                "|All files (*.*)|*.*";
            dialog.AddExtension = true;
            dialog.DefaultExt = "*.xml";
        }
    }
}
