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
    public struct SsaoSettings
    {
        #region Fields

        public float MapScale;
        public float FarPlaneDistance;
        public int BlurRadius;
        public float BlurAmount;
        public float TotalStrength;
        public float Strength;
        public float Falloff;
        public float Radius;

        #endregion

        #region Predefined

        public static SsaoSettings Default
        {
            get
            {
                return new SsaoSettings()
                {
                    MapScale = 0.25f,
                    FarPlaneDistance = 100.0f,
                    BlurRadius = 1,
                    BlurAmount = 2.0f,
                    TotalStrength = 1.0f,
                    Strength = 1.0f,
                    Falloff = 0.0001f,
                    Radius = 8.0f
                };
            }
        }

        #endregion
    }
}
