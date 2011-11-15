#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    public sealed class VolumeFogActorModel : AssetModelActorModel
    {
        #region Fields and Properties

        Vector3 fogColor = Vector3.One;
        public Vector3 FogColor
        {
            get { return fogColor; }
            set
            {
                if (fogColor != value)
                {
                    fogColor = value;
                }
            }
        }

        float fogScale = 0.1f;
        public float FogScale
        {
            get { return fogScale; }
            set
            {
                if (fogScale != value)
                {
                    fogScale = value;
                }
            }
        }

        #endregion
    }
}
