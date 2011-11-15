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
    public struct MonochromeSettings
    {
        #region Fields

        public Vector2 CbCr;

        #endregion

        #region Predefined

        public static Vector2 Grayscale
        {
            get { return new Vector2(0.0f, 0.0f); }
        }

        public static Vector2 SepiaTone
        {
            get { return new Vector2(-0.1f, 0.1f); }
        }

        public static MonochromeSettings Default
        {
            get
            {
                return new MonochromeSettings()
                {
                    CbCr = Grayscale
                };
            }
        }

        #endregion
    }
}
