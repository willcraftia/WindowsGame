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
    public struct ActionSound
    {
        #region Fields

        public ActionSoundKey Key;

        [ContentSerializer(AllowNull = false)]
        public string SoundName;

        #endregion

        #region Constructors

        public ActionSound(
            string actionName,
            string materialName,
            string soundName)
        {
            Key = new ActionSoundKey(actionName, materialName);
            SoundName = soundName;
        }

        public ActionSound(
            ActionSoundKey key,
            string soundName)
        {
            Key = key;
            SoundName = soundName;
        }

        #endregion
    }
}
