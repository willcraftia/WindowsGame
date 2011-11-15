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

namespace Willcraftia.Xna.Framework.Audio
{
    public struct ActionSoundKey
    {
        #region Fields

        [ContentSerializer(AllowNull = false)]
        public string ActionName;

        [DefaultValue(null)]
        [ContentSerializer(Optional = true)]
        public string MaterialName;

        #endregion

        #region Constructors

        public ActionSoundKey(string actionName, string materialName)
        {
            ActionName = actionName;
            MaterialName = materialName;
        }

        #endregion
    }
}
