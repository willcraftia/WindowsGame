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
    public struct GodRaySettings
    {
        #region Fields

        public float MapScale;
        public float Density;
        public float Weight;
        public float Decay;
        public float Exposure;
        public int SampleCount;

        #endregion

        #region Predefined

        public static GodRaySettings Default
        {
            get
            {
                return new GodRaySettings()
                {
                    MapScale = 0.25f,
                    Density = 0.8f,
                    Weight = 0.9f,
                    Decay = 0.8f,
                    Exposure = 0.5f,
                    SampleCount = 32
                };
            }
        }

        #endregion
    }
}
