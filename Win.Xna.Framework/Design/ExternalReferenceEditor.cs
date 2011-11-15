#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;

#endregion

namespace Willcraftia.Win.Xna.Framework.Design
{
    public class ExternalReferenceEditor<T> : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            InitializeDialog(dialog);

            var externalReference = value as ExternalReference<T>;

            if (externalReference != null)
            {
                dialog.FileName = externalReference.Filename;
            }

            if (dialog.ShowDialog() == true)
            {
                if (externalReference != null)
                {
                    externalReference.Filename = dialog.FileName;
                    return externalReference;
                }
                else
                {
                    return new ExternalReference<T>(dialog.FileName);
                }
            }
            else
            {
                return new ExternalReference<T>();
            }
        }

        protected virtual void InitializeDialog(Microsoft.Win32.OpenFileDialog dialog)
        {
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
        }
    }
}
