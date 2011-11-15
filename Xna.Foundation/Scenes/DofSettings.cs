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
    public struct DofSettings
    {
        #region Fields

        public float MapScale;
        public float FarPlaneDistance;
        public int BlurRadius;
        public float BlurAmount;

        public bool FocusOverrideEnabled;
        public float FocusRange;
        public float FocusDistance;

        #endregion

        #region Predefined

        public static DofSettings Default
        {
            get
            {
                return new DofSettings()
                {
                    MapScale = 0.5f,
                    FarPlaneDistance = 300.0f,
                    BlurRadius = 1,
                    BlurAmount = 2.0f,
                    FocusOverrideEnabled = false,
                    FocusRange = 3.0f,
                    FocusDistance = 300.0f
                };
            }
        }

        #endregion
    }
}
