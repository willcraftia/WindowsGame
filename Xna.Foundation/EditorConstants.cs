#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation
{
    public sealed class EditorConstants
    {
        //
        // .NET Framework の Assembly の PublicKeyToken は以下で参照できます。
        //
        // C:\Program Files\Microsoft Visual Studio 10.0\Common7\IDE\VCSExpress.exe.config
        //

        #region Fields and Properties

        /// <summary>
        /// System.Windows.Forms.Design.FileNameEditor の完全修飾名。
        /// </summary>
        public const string FileNameEditorTypeName = "System.Windows.Forms.Design.FileNameEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

        /// <summary>
        /// System.Drawing.Design.UITypeEditor の完全修飾名。
        /// </summary>
        public const string UITypeEditorTypeName = "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

        #endregion
    }
}
