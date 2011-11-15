#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    public sealed class Player
    {
        public CharacterActor Actor { get; set; }

        PlayerIndex controllingPlayer;
        public PlayerIndex ControllingPlayer
        {
            get { return controllingPlayer; }
            set { controllingPlayer = value; }
        }
    }
}
