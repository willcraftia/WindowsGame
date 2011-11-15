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
    public struct BloomSettings
    {
        #region Fields

        public const float DefaultMapScale = 0.25f;
        public const int DefaultBlurRadius = 1;

        public float MapScale;
        public float Threshold;

        public int BlurRadius;
        public float BlurAmount;

        public float BloomIntensity;
        public float BaseIntensity;
        public float BloomSaturation;
        public float BaseSaturation;

        #endregion

        #region Predefined

        public static BloomSettings Default
        {
            get
            {
                return new BloomSettings()
                {
                    MapScale = DefaultMapScale,
                    Threshold = 0.25f,
                    BlurRadius = DefaultBlurRadius,
                    BlurAmount = 4.0f,
                    BloomIntensity = 1.25f,
                    BaseIntensity = 1.0f,
                    BloomSaturation = 1.0f,
                    BaseSaturation = 1.0f
                };
            }
        }

        public static BloomSettings Soft
        {
            get
            {
                return new BloomSettings()
                {
                    MapScale = DefaultMapScale,
                    Threshold = 0.0f,
                    BlurRadius = DefaultBlurRadius,
                    BlurAmount = 3.0f,
                    BloomIntensity = 1.0f,
                    BaseIntensity = 1.0f,
                    BloomSaturation = 1.0f,
                    BaseSaturation = 1.0f
                };
            }
        }

        public static BloomSettings Desaturated
        {
            get
            {
                return new BloomSettings()
                {
                    MapScale = DefaultMapScale,
                    Threshold = 0.5f,
                    BlurRadius = DefaultBlurRadius,
                    BlurAmount = 8.0f,
                    BloomIntensity = 2.0f,
                    BaseIntensity = 1.0f,
                    BloomSaturation = 0.0f,
                    BaseSaturation = 1.0f
                };
            }
        }

        public static BloomSettings Blurry
        {
            get
            {
                return new BloomSettings()
                {
                    MapScale = DefaultMapScale,
                    Threshold = 0.0f,
                    BlurRadius = DefaultBlurRadius,
                    BlurAmount = 2.0f,
                    BloomIntensity = 1.0f,
                    BaseIntensity = 0.1f,
                    BloomSaturation = 1.0f,
                    BaseSaturation = 1.0f
                };
            }
        }

        public static BloomSettings Subtle
        {
            get
            {
                return new BloomSettings()
                {
                    MapScale = DefaultMapScale,
                    Threshold = 0.5f,
                    BlurRadius = DefaultBlurRadius,
                    BlurAmount = 2.0f,
                    BloomIntensity = 1.0f,
                    BaseIntensity = 1.0f,
                    BloomSaturation = 1.0f,
                    BaseSaturation = 1.0f
                };
            }
        }

        #endregion
    }
}
