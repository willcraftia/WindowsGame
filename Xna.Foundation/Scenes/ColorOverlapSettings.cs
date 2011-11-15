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

namespace Willcraftia.Xna.Foundation.Scenes
{
    public struct ColorOverlapSettings
    {
        #region Fields

        public Vector3 Color;
        public float Alpha;

        #endregion

        #region Predefined

        public static ColorOverlapSettings Default
        {
            get
            {
                return new ColorOverlapSettings()
                {
                    Color = Vector3.One,
                    Alpha = 0
                };
            }
        }

        #endregion
    }
}
